using ItemManager.Core.Interfaces;
using System.Linq;

namespace ItemManager.Infrastructure.Services
{
    public class VariantCodeService : IVariantCodeService
    {
        public string SuggestCode(string parentItemCode, IEnumerable<string> abbreviations)
        {
            if (string.IsNullOrWhiteSpace(parentItemCode))
                return string.Empty;

            var cleanedParent = parentItemCode.Trim().ToUpperInvariant();

            var parts = abbreviations?
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim().ToUpperInvariant())
                .ToList();

            if (parts == null || parts.Count == 0)
                return cleanedParent;

            var joined = string.Join("-", parts);

            return $"{cleanedParent}-{joined}";
        }
    }
}