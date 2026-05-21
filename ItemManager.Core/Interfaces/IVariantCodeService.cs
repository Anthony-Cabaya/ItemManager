namespace ItemManager.Core.Interfaces
{
    public interface IVariantCodeService
    {
        string SuggestCode(string parentItemCode, IEnumerable<string> abbreviations);
    }
}