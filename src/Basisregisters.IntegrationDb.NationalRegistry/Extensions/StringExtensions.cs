namespace Basisregisters.IntegrationDb.NationalRegistry.Extensions
{
    using System.Globalization;
    using System.Text;

    public static class StringExtensions
    {
        // Removing diacritics is not made into a sanitizer because we want to do it with every matching attempt.
        public static string RemoveDiacritics(this string text)
        {
            var stringBuilder = new StringBuilder(text.Length);
            var normalizedString = text.Normalize(NormalizationForm.FormD);

            foreach (var character in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(character);
                }
            }

            var resultString = stringBuilder.ToString();

            return resultString.Normalize(NormalizationForm.FormC);
        }

        public static string RemoveQuotations(this string text)
        {
            return text
                .Replace("\'", string.Empty)
                .Replace("\"", string.Empty);
        }

        public static string RemoveSpecialCharacters(this string text)
        {
            return text
                .Replace("°", string.Empty);
        }
    }
}
