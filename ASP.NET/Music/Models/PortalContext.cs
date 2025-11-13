using Microsoft.EntityFrameworkCore;
using MusicPortal.Models;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;

namespace MusicPortal.Data
{
    public class MusicPortalContext : DbContext
    {
        public MusicPortalContext(DbContextOptions<MusicPortalContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Song> Songs { get; set; }

        public static void HashPassword(string password, out string hashedPassword, out string salt)
        {

            byte[] saltbuf = new byte[16];
            RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(saltbuf);

            StringBuilder sb = new StringBuilder(16);
            for (int i = 0; i < 16; i++)
                sb.Append(string.Format("{0:X2}", saltbuf[i]));
            salt = sb.ToString();

            byte[] saltedPassword = Encoding.Unicode.GetBytes(salt + password);

            byte[] byteHash = SHA256.HashData(saltedPassword);

            StringBuilder hash = new StringBuilder(byteHash.Length);
            for (int i = 0; i < byteHash.Length; i++)
                hash.Append(string.Format("{0:X2}", byteHash[i]));

            hashedPassword = hash.ToString();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Song>()
                .HasOne(s => s.Genre)
                .WithMany(g => g.Songs)
                .HasForeignKey(s => s.GenreId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Song>()
                .HasOne(s => s.User)
                .WithMany(u => u.Songs)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            HashPassword("admin123", out string password, out string salt);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@gmail.com",
                    Password = password,
                    IsActive = true,
                    IsAdmin = true,
                    Salt = salt
                }
            );

            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = 1, Name = "Rock", Description = "Rock music" },
                new Genre { Id = 2, Name = "Pop", Description = "Popular music" },
                new Genre { Id = 3, Name = "Jazz", Description = "Jazz music" },
                new Genre { Id = 4, Name = "Classical", Description = "Classical music" }
            );
        }
    }
}