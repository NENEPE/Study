using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace MoviesRazor.Models
{
    public class MovieContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
        public MovieContext(DbContextOptions<MovieContext> options)
           : base(options)
        {
            if (Database.EnsureCreated())
            {
                Movies?.Add(new Movie { Title = "Avengers", Director = "Josh Whedon", Desc = "In the film, Nick Fury and the spy agency S.H.I.E.L.D. recruit Tony Stark, Steve Rogers, Bruce Banner, Thor, Natasha Romanoff, and Clint Barton to form a team capable of stopping Thor's brother Loki from subjugating Earth.", Genre = "Superhero fiction", ReleaseYear = 2012 , Poster = "\\img\\avengers.png"});
                Movies?.Add(new Movie { Title = "Shrek", Director = "Andrew Adamson", Desc = "A mean lord exiles fairytale creatures to the swamp of a grumpy ogre, who must go on a quest and rescue a princess for the lord in order to get his land back.", Genre = "Fairy Tale Comedy", ReleaseYear = 2001, Poster = "\\img\\shrek.png" });
                Movies?.Add(new Movie { Title = "Martian", Director = "Ridley Scott", Desc = "An astronaut becomes stranded on Mars after his team assumes him dead, and must rely on his ingenuity to find a way to signal to Earth that he is alive and can survive until a potential rescue.", Genre = "Science fiction", ReleaseYear = 2015, Poster = "\\img\\martian.png" });
                Movies?.Add(new Movie { Title = "Cars", Director = "John Lasseter", Desc = "On the way to the biggest race of his life, a hotshot rookie race car gets stranded in a rundown town and learns that winning isn't everything in life.", Genre = "Adventure", ReleaseYear = 2006, Poster = "\\img\\cars.png" });
                Movies?.Add(new Movie { Title = "Spider-Man: Into the Spider-Verse", Director = "Bob Persichetti", Desc = "Teen Miles Morales becomes the Spider-Man of his universe and must join with five spider-powered individuals from other dimensions to stop a threat for all realities.", Genre = "Superhero fiction", ReleaseYear = 2018, Poster = "\\img\\itsv.png" });
                Movies?.Add(new Movie { Title = "Catch Me If You Can", Director = "Steven Spielberg", Desc = "Barely 17 yet, Frank is a skilled forger who has passed as a doctor, lawyer and pilot. FBI agent Carl becomes obsessed with tracking down the con man, who only revels in the pursuit.", Genre = "Docudrama", ReleaseYear = 2002, Poster = "\\img\\catch.png" });
                Movies?.Add(new Movie { Title = "Rush Hour", Director = "Brett Ratner", Desc = "A loyal and dedicated Hong Kong Inspector teams up with a reckless and loudmouthed L.A.P.D. detective to rescue the Chinese Consul's kidnapped daughter, while trying to arrest a dangerous crime lord along the way.", Genre = "Buddy Comedy", ReleaseYear = 1998, Poster = "\\img\\rushour.png" });
                Movies?.Add(new Movie { Title = "Saw", Director = "James Wan", Desc = "Two men awaken to find themselves on the opposite sides of a dead body, each with specific instructions to kill the other, escape or face the consequences. These two are the latest contestants in Jigsaw's games.", Genre = "Body Horror", ReleaseYear = 2004, Poster = "\\img\\saw.png" });
                Movies?.Add(new Movie { Title = "Now You See Me", Director = "Louis Leterrier", Desc = "An FBI agent and an Interpol detective track a team of illusionists who pull off bank heists during their performances, and reward their audiences with the money.", Genre = "Heist", ReleaseYear = 2013, Poster = "\\img\\nysm.png" });
                Movies?.Add(new Movie { Title = "In Time", Director = "Andrew Niccol", Desc = "In a future where people stop aging at 25, but are engineered to live only one more year, having the means to buy your way out of the situation is a shot at immortal youth. Will Salas is accused of murder and on the run with a hostage.", Genre = "Cyber Thriller", ReleaseYear = 2011, Poster = "\\img\\time.png" });
                Movies?.Add(new Movie { Title = "Pulp Fiction", Director = "Quentin Tarantino", Desc = "The lives of two mob hitmen, a boxer, a gangster and his wife, and a pair of diner bandits intertwine in four tales of violence and redemption.", Genre = "Dark Comedy", ReleaseYear = 1994, Poster = "\\img\\pulpfiction.png" });
                SaveChanges();
            }
        }
    }
}
