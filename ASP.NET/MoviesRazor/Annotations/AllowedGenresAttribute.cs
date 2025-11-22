using System.ComponentModel.DataAnnotations;

namespace MoviesRazor.Annotations
{
    public class AllowedGenresAttribute : ValidationAttribute
    {
        private readonly string[] allowedGenres;

        public AllowedGenresAttribute(string[] ag)
        {
            allowedGenres = ag;
        }

        public override bool IsValid(object? value)
        {
            if (value != null)
            {
                string? strval = value.ToString();
                for (int g = 0; g < allowedGenres.Length; g++)
                {
                    if (strval == allowedGenres[g])
                        return true;
                }
            }
            return false;
        }
    }
}
