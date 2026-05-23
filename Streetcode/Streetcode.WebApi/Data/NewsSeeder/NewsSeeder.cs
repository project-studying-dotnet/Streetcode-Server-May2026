using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.News;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.NewsSeeder
{
    [ExcludeFromCodeCoverage]
    public static class NewsSeeder
    {
        public static async Task FillSeedAsync(
            StreetcodeDbContext context)
        {
        var entities = new List<News>
        {
            new()
            {
                Title = "27 квітня встановлюємо перший стріткод!",
                Text = "<p>Встановлення таблички про Михайла Грушевського в м. Київ стало важливою подією для киян та гостей столиці. Вона не лише прикрашає вулицю міста, а й нагадує про значний внесок цієї визначної особистості в історію України. Це також сприяє розповсюдженню знань про Михайла Грушевського серед широкого загалу, виховує національну свідомість та гордість за власну культуру.\r\n\r\nВстановлення таблички про Михайла Грушевського в Києві є важливим кроком на шляху вшанування відомих особистостей, які внесли вагомий внесок у розвиток України. Це також показує, що в Україні дбають про збереження національної спадщини та визнання внеску видатних історичних постатей в формування національної ідентичності.\r\n\r\nУрочисте встановлення таблички про Михайла Грушевського відбулося за участі високопосадовців міста, представників наукової спільноти та громадськості. Під час церемонії відбулися промови, в яких відзначили важливість дослідницької та літературної діяльності М. Грушевського, його внесок у вивчення історії України та роль у національному відродженні.\r\n\r\nМихайло Грушевський жив і працював в Києві на початку ХХ століття. Він був визнаний одним з провідних істориків свого часу, який досліджував історію України з наукової та національної позицій. Його праці були визнані авторитетними не лише в Україні, але й у світі, і мають велике значення для розуміння минулого та формування майбутнього українського народу.\r\n\r\nТабличка з відтвореним зображенням Михайла Грушевського стала вагомим символом вшанування цієї видатної постаті. Вона стала візитівкою Києва та пам'яткою культурної спадщини України, яка привертає увагу мешканців та гостей міста. Це важливий крок на шляху до збереження національної історії, культури та національної свідомості в Україні.\r\n\r\nВстановлення таблички про Михайла Грушевського в Києві свідчить про важливість визнання історичної спадщини та внеску видатних постатей в національну свідомість. Це також є визнанням ролі М. Грушевського у формуванні української національної ідентичності та його внеску в розвиток наукової та культурної спадщини України.\r\n\r\nТабличка була встановлена на видному місці в центрі Києва, недалеко від місця, де розташовується будинок, в якому колись проживав Михайло Грушевський. Зображення на табличці передає фотографію видатного історика, а також містить кратку інформацію про його життя та діяльність.\r\n\r\nМешканці та гості Києва високо оцінюють встановлення таблички про Михайла Грушевського, яке стало ще одним кроком на шляху до вшанування історичної спадщини України. Це також важливий крок у визнанні ролі українських науковців та культурних діячів у світовому контексті.\r\n\r\nВстановлення таблички про Михайла Грушевського в Києві є однією з ініціатив, спрямованих на підтримку і розширення національної пам'яті та відтворення історичної правди. Це важливий крок на шляху до відродження національної свідомості та підкреслення значення української культурної спадщини в світовому контексті.</p>",
                URL = "first-streetcode",
                ImageId = 24,
                CreationDate = DateTime.Now,
            },
            new()
            {
                Title = "Новий учасник команди!",
                Text = "<p>Привітаймо нового учасника команди - Терентьєва Даниїла!. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque arcu orci, dictum at posuere a, tincidunt sit amet nibh. Donec pellentesque ac mauris tristique egestas. Vestibulum hendrerit eget nisi non viverra. Nullam ultricies sapien ac ipsum ullamcorper tristique. Mauris auctor, sapien vitae molestie ornare, libero orci fringilla velit, sed pharetra nibh augue id tellus. Mauris pulvinar vel felis convallis molestie. Integer mauris felis, ultrices nec vestibulum at, ullamcorper eu massa. Proin posuere consectetur facilisis. Nunc volutpat dictum massa, ac volutpat nisl malesuada nec.\r\n\r\nNulla nec felis quis metus efficitur efficitur ac nec est. Nulla eros quam, tincidunt at elit nec, iaculis eleifend sem. Pellentesque id sem id erat mollis fermentum non at ipsum. Donec justo ante, commodo a pharetra a, consectetur at urna. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Praesent porta, odio sed venenatis posuere, felis nibh finibus dui, placerat molestie dui libero at nisi. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Duis quis nisi in nisi pulvinar ultrices.\r\n\r\nPellentesque ante nunc, mattis vitae iaculis id, sollicitudin nec tortor. Pellentesque eu lectus suscipit, sodales nunc eu, lobortis enim. Praesent tempus dolor et felis vulputate hendrerit. Nunc ut lacus.</p>",
                URL = "danya",
                ImageId = 28,
                CreationDate = DateTime.Now,
            },
            new()
            {
                Title = "Новий учасник команди!",
                Text = "<p>Привітаймо нового учасника команди - Скам Мастера!. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque arcu orci, dictum at posuere a, tincidunt sit amet nibh. Donec pellentesque ac mauris tristique egestas. Vestibulum hendrerit eget nisi non viverra. Nullam ultricies sapien ac ipsum ullamcorper tristique. Mauris auctor, sapien vitae molestie ornare, libero orci fringilla velit, sed pharetra nibh augue id tellus. Mauris pulvinar vel felis convallis molestie. Integer mauris felis, ultrices nec vestibulum at, ullamcorper eu massa. Proin posuere consectetur facilisis. Nunc volutpat dictum massa, ac volutpat nisl malesuada nec.\r\n\r\nNulla nec felis quis metus efficitur efficitur ac nec est. Nulla eros quam, tincidunt at elit nec, iaculis eleifend sem. Pellentesque id sem id erat mollis fermentum non at ipsum. Donec justo ante, commodo a pharetra a, consectetur at urna. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Praesent porta, odio sed venenatis posuere, felis nibh finibus dui, placerat molestie dui libero at nisi. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Duis quis nisi in nisi pulvinar ultrices.\r\n\r\nPellentesque ante nunc, mattis vitae iaculis id, sollicitudin nec tortor. Pellentesque eu lectus suscipit, sodales nunc eu, lobortis enim. Praesent tempus dolor et felis vulputate hendrerit. Nunc ut lacus.</p>",
                URL = "scum",
                ImageId = 29,
                CreationDate = DateTime.Now,
            }
        };

        await context.News.SeedIfEmptyAsync(
            entities,
            context);
        }
    }
}