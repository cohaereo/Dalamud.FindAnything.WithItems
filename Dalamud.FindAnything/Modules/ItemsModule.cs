using Dalamud.Interface.Textures;
using Dalamud.Utility;
using Lumina.Excel.Sheets;

namespace Dalamud.FindAnything.Modules;

public class ItemsModule : SearchModule
{
    public override Configuration.SearchSetting SearchSetting  => Configuration.SearchSetting.Items;
    
    public override void Search(SearchContext ctx, Normalizer normalizer, FuzzyMatcher matcher, GameState gameState)
    {
        var items = Service.Data.GetExcelSheet<Item>();
        foreach (var (itemId, entry) in FindAnythingPlugin.SearchDatabase.GetAll<Item>())
        {
            var item = items.GetRowOrDefault(itemId);
            if (item == null || item.Value.RowId == 0) {
                continue;
            }
            
            var score = matcher.Matches(entry.Searchable);
            if (score > 0)
                ctx.AddResult(new ItemsResult {
                    Score = score * Weight,
                    Name = entry.Display,
                    Item = item.Value,
                    Icon = FindAnythingPlugin.TexCache.GetIcon(item.Value.Icon),
                });
        }
    }
    
    private class ItemsResult : ISearchResult {
        public string CatName => "Item";
        public required string Name { get; init; }
        public required ISharedImmediateTexture? Icon { get; init; }
        public required int Score { get; init; }
        public required Item Item { get; init; }

        public object Key => Item.RowId;

        public unsafe void Selected() {
            var id = ItemUtil.GetBaseId(Item.RowId).ItemId;
            if (id > 0) {
                Service.CommandManager.ProcessCommand("/moreinfo " + id);
            }
        }
        
    }
}