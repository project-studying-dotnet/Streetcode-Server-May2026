using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.WebApi.InitialData.SeederExtensions;
using Streetcode.DAL.Persistence;

namespace Streetcode.WebApi.InitialData.TermsSeeder
{
    public static class TermsSeeder
    {
        public static async Task FillSeedAsync(
            StreetcodeDbContext context)
        {
        var terms = new List<Term>
        {
            new()
            {
                Title = "етнограф",
                Description = "Етнографія — суспільствознавча наука, об'єктом дослідження якої є народи, їхня культура і побут, походження, розселення," +
                " процеси культурно-побутових відносин на всіх етапах історії людства."
            },
            new()
            {
                Title = "гравер",
                Description = "Гра́фіка — вид образотворчого мистецтва, для якого характерна перевага ліній і штрихів, використання контрастів білого та" +
                    " чорного та менше, ніж у живописі, використання кольору. Твори можуть мати як монохромну, так і поліхромну гаму."
            },
            new()
            {
                Title = "кріпак",
                Description = "Кріпа́цтво, або кріпосне́ право, у вузькому сенсі — правова система, або система правових норм при феодалізмі, яка встановлювала" +
                    " залежність селянина від феодала й неповну власність феодала на селянина."
            },
            new()
            {
                Title = "мачуха",
                Description = "Ма́чуха — нерідна матір для дітей чоловіка від його попереднього шлюбу.",
            }
        };

        await context.Terms.SeedIfEmptyAsync(
            terms,
            context);
        }
    }
}
