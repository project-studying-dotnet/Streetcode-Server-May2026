// <copyright file="WebParsingUtilsTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Streetcode.XUnitTest.WebApi.Utils
{
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore.Query;
    using Moq;
    using Streetcode.DAL.Entities.Toponyms;
    using Streetcode.DAL.Repositories.Interfaces.Base;
    using Streetcode.WebApi.Utils;
    using Xunit;

    /// <summary>
    /// Contains tests for <see cref="WebParsingUtils"/>.
    /// </summary>
    public class WebParsingUtilsTests
    {
        private const string ValidUrl = "https://example.com/file.zip";
        private static readonly string ValidZipPath = Path.Combine(Path.GetTempPath(), "houses.zip");
        private static readonly string ValidExtractTo = Path.GetTempPath();

        static WebParsingUtilsTests()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        /// <summary>
        /// Should throw <see cref="ArgumentException"/> when <paramref name="fileUrl"/> is null.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task DownloadAndExtractAsync_ShouldThrowArgumentException_WhenFileUrlIsNull()
        {
            // Arrange
            var act = async () => await WebParsingUtils.DownloadAndExtractAsync(
                null!, ValidZipPath, ValidExtractTo, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ArgumentException>().WithParameterName("fileUrl");
        }

        /// <summary>
        /// Should throw <see cref="ArgumentException"/> when <paramref name="fileUrl"/> is empty.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task DownloadAndExtractAsync_ShouldThrowArgumentException_WhenFileUrlIsEmpty()
        {
            // Arrange
            var act = async () => await WebParsingUtils.DownloadAndExtractAsync(
                string.Empty, ValidZipPath, ValidExtractTo, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ArgumentException>().WithParameterName("fileUrl");
        }

        /// <summary>
        /// Should throw <see cref="ArgumentException"/> when <paramref name="fileUrl"/> is not a valid URI.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task DownloadAndExtractAsync_ShouldThrowArgumentException_WhenFileUrlIsInvalidUri()
        {
            // Arrange
            var act = async () => await WebParsingUtils.DownloadAndExtractAsync(
                "not-a-valid-url", ValidZipPath, ValidExtractTo, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ArgumentException>().WithParameterName("fileUrl");
        }

        /// <summary>
        /// Should throw <see cref="ArgumentException"/> when <paramref name="zipPath"/> is null.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task DownloadAndExtractAsync_ShouldThrowArgumentException_WhenZipPathIsNull()
        {
            // Arrange
            var act = async () => await WebParsingUtils.DownloadAndExtractAsync(
                ValidUrl, null!, ValidExtractTo, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ArgumentException>().WithParameterName("zipPath");
        }

        /// <summary>
        /// Should throw <see cref="ArgumentException"/> when <paramref name="zipPath"/> is empty.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task DownloadAndExtractAsync_ShouldThrowArgumentException_WhenZipPathIsEmpty()
        {
            // Arrange
            var act = async () => await WebParsingUtils.DownloadAndExtractAsync(
                ValidUrl, string.Empty, ValidExtractTo, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ArgumentException>().WithParameterName("zipPath");
        }

        /// <summary>
        /// Should throw <see cref="ArgumentException"/> when <paramref name="extractTo"/> is null.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task DownloadAndExtractAsync_ShouldThrowArgumentException_WhenExtractToIsNull()
        {
            // Arrange
            var act = async () => await WebParsingUtils.DownloadAndExtractAsync(
                ValidUrl, ValidZipPath, null!, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ArgumentException>().WithParameterName("extractTo");
        }

        /// <summary>
        /// Should throw <see cref="ArgumentException"/> when <paramref name="extractTo"/> is empty.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task DownloadAndExtractAsync_ShouldThrowArgumentException_WhenExtractToIsEmpty()
        {
            // Arrange
            var act = async () => await WebParsingUtils.DownloadAndExtractAsync(
                ValidUrl, ValidZipPath, string.Empty, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ArgumentException>().WithParameterName("extractTo");
        }

        /// <summary>
        /// Should not throw <see cref="ArgumentException"/> when all arguments are valid absolute paths.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task DownloadAndExtractAsync_ShouldNotThrowArgumentException_WhenAllArgumentsAreValid()
        {
            // Arrange
            var act = async () => await WebParsingUtils.DownloadAndExtractAsync(
                ValidUrl, ValidZipPath, ValidExtractTo, CancellationToken.None);

            // Act & Assert
            await act.Should().NotThrowAsync<ArgumentException>();
        }

        /// <summary>
        /// Verifies that the <see cref="WebParsingUtils"/> constructor creates a non-null instance
        /// when a valid <see cref="IRepositoryWrapper"/> is supplied.
        /// </summary>
        [Fact]
        public void Constructor_ShouldCreateInstance_WhenRepositoryIsProvided()
        {
            var repoMock = new Mock<IRepositoryWrapper>();

            var utils = new WebParsingUtils(repoMock.Object);

            utils.Should().NotBeNull();
        }

        /// <summary>
        /// Verifies that <see cref="WebParsingUtils.SaveToponymsToDbAsync"/> calls
        /// <c>ToponymRepository.DeleteRange</c> exactly once before inserting new toponyms.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task SaveToponymsToDbAsync_ShouldDeleteAllExistingToponyms_BeforeInserting()
        {
            var csvPath = CreateTempCsvWithDataRows(
                "Region;AdminOld;AdminNew;Gromada;Community;col5;вул. Шевченка;50.4501;30.5234");
            var repoMock = CreateRepositoryMock();

            try
            {
                var utils = new WebParsingUtils(repoMock.Object);
                await utils.SaveToponymsToDbAsync(csvPath);

                repoMock.Verify(r => r.ToponymRepository.DeleteRange(It.IsAny<IEnumerable<Toponym>>()), Times.Once);
            }
            finally
            {
                File.Delete(csvPath);
            }
        }

        /// <summary>
        /// Verifies that <see cref="WebParsingUtils.SaveToponymsToDbAsync"/> calls
        /// <c>ToponymRepository.CreateAsync</c> once per valid data row in the CSV file.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task SaveToponymsToDbAsync_ShouldCreateToponym_ForEachValidRow()
        {
            var csvPath = CreateTempCsvWithDataRows(
                "Region1;AdminOld;AdminNew;Gromada;Community;col5;вул. Шевченка;50.4501;30.5234",
                "Region2;AdminOld;AdminNew;Gromada;Community;col5;пров. Польовий;49.0;32.0");
            var repoMock = CreateRepositoryMock();

            try
            {
                var utils = new WebParsingUtils(repoMock.Object);
                await utils.SaveToponymsToDbAsync(csvPath);

                repoMock.Verify(r => r.ToponymRepository.CreateAsync(It.IsAny<Toponym>()), Times.Exactly(2));
            }
            finally
            {
                File.Delete(csvPath);
            }
        }

        /// <summary>
        /// Verifies that <see cref="WebParsingUtils.SaveToponymsToDbAsync"/> calls
        /// <c>SaveChangesAsync</c> at least once after processing all rows.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task SaveToponymsToDbAsync_ShouldSaveChanges_AfterAllInsertions()
        {
            var csvPath = CreateTempCsvWithDataRows(
                "Region;AdminOld;AdminNew;Gromada;Community;col5;вул. Шевченка;50.4501;30.5234");
            var repoMock = CreateRepositoryMock();

            try
            {
                var utils = new WebParsingUtils(repoMock.Object);
                await utils.SaveToponymsToDbAsync(csvPath);

                repoMock.Verify(r => r.SaveChangesAsync(), Times.AtLeastOnce);
            }
            finally
            {
                File.Delete(csvPath);
            }
        }

        /// <summary>
        /// Verifies that <see cref="WebParsingUtils.SaveToponymsToDbAsync"/> does not call
        /// <c>ToponymRepository.CreateAsync</c> when the CSV file contains only a header row.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task SaveToponymsToDbAsync_ShouldNotCreateAnyToponym_WhenCsvHasOnlyHeader()
        {
            var csvPath = CreateTempCsvWithDataRows();
            var repoMock = CreateRepositoryMock();

            try
            {
                var utils = new WebParsingUtils(repoMock.Object);
                await utils.SaveToponymsToDbAsync(csvPath);

                repoMock.Verify(r => r.ToponymRepository.CreateAsync(It.IsAny<Toponym>()), Times.Never);
            }
            finally
            {
                File.Delete(csvPath);
            }
        }

        /// <summary>
        /// Verifies that <see cref="WebParsingUtils.SaveToponymsToDbAsync"/> silently skips a row whose
        /// coordinate columns cannot be parsed as decimals and continues processing subsequent valid rows.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task SaveToponymsToDbAsync_ShouldSkipInvalidRow_AndContinueWithValidOnes()
        {
            var csvPath = CreateTempCsvWithDataRows(
                "Region;AdminOld;AdminNew;Gromada;Community;col5;вул. Погана;NOT_A_DECIMAL;30.5234",
                "Region;AdminOld;AdminNew;Gromada;Community;col5;вул. Центральна;50.4501;30.5234");
            var repoMock = CreateRepositoryMock();

            try
            {
                var utils = new WebParsingUtils(repoMock.Object);
                var act = async () => await utils.SaveToponymsToDbAsync(csvPath);

                await act.Should().NotThrowAsync();
                repoMock.Verify(r => r.ToponymRepository.CreateAsync(It.IsAny<Toponym>()), Times.Once);
            }
            finally
            {
                File.Delete(csvPath);
            }
        }

        /// <summary>
        /// Verifies that <see cref="WebParsingUtils.SaveToponymsToDbAsync"/> maps every CSV column
        /// to the corresponding <see cref="Toponym"/> property, including street type resolution.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task SaveToponymsToDbAsync_ShouldSetToponymFields_FromCsvRow()
        {
            var csvPath = CreateTempCsvWithDataRows(
                "MyRegion;OldAdmin;NewAdmin;MyGromada;MyCommunity;col5;вул. Хрещатик;50.4501;30.5234");
            var repoMock = CreateRepositoryMock();
            Toponym? captured = null;
            repoMock.Setup(r => r.ToponymRepository.CreateAsync(It.IsAny<Toponym>()))
                .Callback<Toponym>(t => captured = t)
                .ReturnsAsync(new Toponym());

            try
            {
                var utils = new WebParsingUtils(repoMock.Object);
                await utils.SaveToponymsToDbAsync(csvPath);

                captured.Should().NotBeNull();
                captured!.Oblast.Should().Be("MyRegion");
                captured.AdminRegionOld.Should().Be("OldAdmin");
                captured.AdminRegionNew.Should().Be("NewAdmin");
                captured.Gromada.Should().Be("MyGromada");
                captured.Community.Should().Be("MyCommunity");
                captured.StreetType.Should().Be("вулиця");
                captured.Coordinate.Latitude.Should().Be(50.4501m);
            }
            finally
            {
                File.Delete(csvPath);
            }
        }

        /// <summary>
        /// Verifies that <see cref="WebParsingUtils.ProcessCsvFileAsync"/> invokes the repository's
        /// <c>DeleteRange</c> and <c>SaveChangesAsync</c> when all rows in <c>houses.csv</c>
        /// are already present in <c>data.csv</c> and no HTTP fetch is required.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task ProcessCsvFileAsync_ShouldCallRepository_WhenAllRowsAlreadyParsed()
        {
            var tempDir = Directory.CreateTempSubdirectory().FullName;
            try
            {
                var dataRow = "Region;AdminOld;AdminNew;Gromada;м.Київ Київ;col5;вул. Шевченка";
                await File.WriteAllLinesAsync(
                    Path.Combine(tempDir, "houses.csv"),
                    new[] { "header", dataRow },
                    Encoding.GetEncoding(1251));

                var parsedRow = "Region;AdminOld;AdminNew;Gromada;м.Київ Київ;col5;вул. Шевченка;50.4501;30.5234";
                await File.WriteAllLinesAsync(
                    $"{tempDir}/data.csv",
                    new[] { "header", parsedRow },
                    Encoding.GetEncoding(1251));

                var repoMock = CreateRepositoryMock();
                var utils = new WebParsingUtils(repoMock.Object);

                await utils.ProcessCsvFileAsync(tempDir);

                repoMock.Verify(r => r.ToponymRepository.DeleteRange(It.IsAny<IEnumerable<Toponym>>()), Times.Once);
                repoMock.Verify(r => r.SaveChangesAsync(), Times.AtLeastOnce);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        /// <summary>
        /// Verifies that <see cref="WebParsingUtils.ProcessCsvFileAsync"/> removes lines from
        /// <c>data.csv</c> whose first seven columns are no longer present in <c>houses.csv</c>.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task ProcessCsvFileAsync_ShouldRemoveStaleRows_WhenDataCsvHasOutdatedEntries()
        {
            var tempDir = Directory.CreateTempSubdirectory().FullName;
            try
            {
                var activeRow = "Region;AdminOld;AdminNew;Gromada;м.Київ Київ;col5;вул. Соборна";
                await File.WriteAllLinesAsync(
                    Path.Combine(tempDir, "houses.csv"),
                    new[] { "header", activeRow },
                    Encoding.GetEncoding(1251));

                var parsedActive = "Region;AdminOld;AdminNew;Gromada;м.Київ Київ;col5;вул. Соборна;50.4501;30.5234";
                var staleRow = "Stale;AdminOld;AdminNew;Gromada;м.Київ Київ;col5;вул. Стара;50.0;30.0";
                var dataCsvPath = $"{tempDir}/data.csv";
                await File.WriteAllLinesAsync(
                    dataCsvPath,
                    new[] { "header", parsedActive, staleRow },
                    Encoding.GetEncoding(1251));

                var repoMock = CreateRepositoryMock();
                var utils = new WebParsingUtils(repoMock.Object);

                await utils.ProcessCsvFileAsync(tempDir);

                var lines = await File.ReadAllLinesAsync(dataCsvPath, Encoding.GetEncoding(1251));
                lines.Should().NotContain(staleRow);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        /// <summary>
        /// Verifies that <see cref="WebParsingUtils.ProcessCsvFileAsync"/> creates <c>data.csv</c>
        /// when it does not yet exist in the target directory.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task ProcessCsvFileAsync_ShouldCreateDataCsv_WhenItDoesNotExist()
        {
            var tempDir = Directory.CreateTempSubdirectory().FullName;
            try
            {
                await File.WriteAllTextAsync(
                    Path.Combine(tempDir, "houses.csv"),
                    string.Empty,
                    Encoding.GetEncoding(1251));

                var repoMock = CreateRepositoryMock();
                var utils = new WebParsingUtils(repoMock.Object);

                await utils.ProcessCsvFileAsync(tempDir);

                File.Exists($"{tempDir}/data.csv").Should().BeTrue();
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        /// <summary>
        /// Verifies that <see cref="WebParsingUtils.ProcessCsvFileAsync"/> deletes the source
        /// <c>houses.csv</c> file when <c>deleteFile</c> is <see langword="true"/>.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task ProcessCsvFileAsync_ShouldDeleteHousesCsv_WhenDeleteFileIsTrue()
        {
            var tempDir = Directory.CreateTempSubdirectory().FullName;
            try
            {
                var dataRow = "Region;AdminOld;AdminNew;Gromada;м.Київ Київ;col5;вул. Шевченка";
                var housesCsvPath = Path.Combine(tempDir, "houses.csv");
                await File.WriteAllLinesAsync(
                    housesCsvPath,
                    new[] { "header", dataRow },
                    Encoding.GetEncoding(1251));

                var parsedRow = "Region;AdminOld;AdminNew;Gromada;м.Київ Київ;col5;вул. Шевченка;50.4501;30.5234";
                await File.WriteAllLinesAsync(
                    $"{tempDir}/data.csv",
                    new[] { "header", parsedRow },
                    Encoding.GetEncoding(1251));

                var repoMock = CreateRepositoryMock();
                var utils = new WebParsingUtils(repoMock.Object);

                await utils.ProcessCsvFileAsync(tempDir, deleteFile: true);

                File.Exists(housesCsvPath).Should().BeFalse();
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        /// <summary>
        /// Verifies that the private <c>OptimizeStreetname</c> method correctly extracts
        /// the street name and resolves the street type for each known Ukrainian street prefix.
        /// </summary>
        /// <param name="input">Raw street name string as it appears in the source CSV.</param>
        /// <param name="expectedName">Expected extracted street name without the prefix.</param>
        /// <param name="expectedType">Expected full street type corresponding to the prefix.</param>
        [Theory]
        [InlineData("вул. Шевченка", "Шевченка", "вулиця")]
        [InlineData("пров. Польовий", "Польовий", "провулок")]
        [InlineData("просп. Перемоги", "Перемоги", "проспект")]
        [InlineData("бульв. Лесі", "Лесі", "бульвар")]
        [InlineData("шосе Одеське", "Одеське", "шосе")]
        public void OptimizeStreetname_ShouldReturnCorrectTypeAndName_ForKnownPrefixes(
            string input, string expectedName, string expectedType)
        {
            var (name, type) = InvokeOptimizeStreetname(input);

            name.Should().Be(expectedName);
            type.Should().Be(expectedType);
        }

        /// <summary>
        /// Verifies that the private <c>OptimizeStreetname</c> method returns the substring after the
        /// second space as the name and <c>"парк"</c> as the type when the input starts with
        /// <c>"жилий масив"</c>.
        /// </summary>
        [Fact]
        public void OptimizeStreetname_ShouldReturnPark_ForZhylyyMasyvPrefix()
        {
            var (name, type) = InvokeOptimizeStreetname("жилий масив Перемога");

            name.Should().Be("Перемога");
            type.Should().Be("парк");
        }

        /// <summary>
        /// Verifies that the private <c>OptimizeStreetname</c> method returns empty strings for both
        /// name and type when the input does not match any known street prefix.
        /// </summary>
        [Fact]
        public void OptimizeStreetname_ShouldReturnEmptyStrings_ForUnknownPrefix()
        {
            var (name, type) = InvokeOptimizeStreetname("Невідома назва вулиці");

            name.Should().BeEmpty();
            type.Should().BeEmpty();
        }

        /// <summary>
        /// Verifies that the private <c>GetDistinctRows</c> method deduplicates rows that share
        /// the same first seven semicolon-separated columns, keeping only one representative per group.
        /// </summary>
        [Fact]
        public void GetDistinctRows_ShouldRemoveDuplicates_BasedOnFirstSevenColumns()
        {
            var rows = new List<string>
            {
                "a;b;c;d;e;f;g;extra1",
                "a;b;c;d;e;f;g;extra2",
                "x;y;z",
            };

            var result = InvokeGetDistinctRows(rows);

            result.Should().HaveCount(2);
            result.Should().Contain("a;b;c;d;e;f;g");
            result.Should().Contain("x;y;z");
        }

        /// <summary>
        /// Verifies that the private <c>GetDistinctRows</c> method returns an empty list
        /// when the input sequence contains no elements.
        /// </summary>
        [Fact]
        public void GetDistinctRows_ShouldReturnEmpty_WhenInputIsEmpty()
        {
            var result = InvokeGetDistinctRows(new List<string>());

            result.Should().BeEmpty();
        }

        /// <summary>
        /// Verifies that the private <c>GetDistinctRows</c> method uses the <c>beforeColumn</c>
        /// parameter to control how many columns are included in the key used for deduplication.
        /// </summary>
        [Fact]
        public void GetDistinctRows_ShouldRespectCustomColumnCount()
        {
            var rows = new List<string> { "a;b;c;d;e;f;g;h", "a;b;c;DIFF;e;f;g;h" };

            var result = InvokeGetDistinctRows(rows, beforeColumn: 3);

            result.Should().HaveCount(1);
            result[0].Should().Be("a;b;c");
        }

        /// <summary>
        /// Verifies that the private <c>ParseJsonToCoordinateTuple</c> method extracts the
        /// <c>lat</c> and <c>lon</c> values from the first element of a valid Nominatim JSON response.
        /// </summary>
        [Fact]
        public void ParseJsonToCoordinateTuple_ShouldReturnCoordinates_WhenJsonIsValid()
        {
            var json = "[{\"lat\":\"50.4501\",\"lon\":\"30.5234\",\"display_name\":\"Київ\"}]";

            var (lat, lon) = InvokeParseJsonToCoordinateTuple(json);

            lat.Should().Be("50.4501");
            lon.Should().Be("30.5234");
        }

        /// <summary>
        /// Verifies that the private <c>ParseJsonToCoordinateTuple</c> method returns the default
        /// tuple <c>(null, null)</c> when the JSON contains an empty array.
        /// </summary>
        [Fact]
        public void ParseJsonToCoordinateTuple_ShouldReturnDefault_WhenJsonArrayIsEmpty()
        {
            var (lat, lon) = InvokeParseJsonToCoordinateTuple("[]");

            lat.Should().BeNull();
            lon.Should().BeNull();
        }

        /// <summary>
        /// Verifies that the private <c>ParseJsonToCoordinateTuple</c> method returns the default
        /// tuple <c>(null, null)</c> when the JSON deserializes to <see langword="null"/>.
        /// </summary>
        [Fact]
        public void ParseJsonToCoordinateTuple_ShouldReturnDefault_WhenJsonIsNull()
        {
            var (lat, lon) = InvokeParseJsonToCoordinateTuple("null");

            lat.Should().BeNull();
            lon.Should().BeNull();
        }

        /// <summary>
        /// Verifies that <see cref="WebParsingUtils.FetchCoordsByAddressAsync"/> returns a tuple
        /// without throwing, regardless of network availability. When the HTTP request succeeds,
        /// the try-block path is exercised; when it fails, the catch block returns <c>(null, null)</c>.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task FetchCoordsByAddressAsync_ShouldReturnTupleWithoutThrowing_ForAnyAddress()
        {
            var act = async () => await WebParsingUtils.FetchCoordsByAddressAsync("Хрещатик, Київ");

            await act.Should().NotThrowAsync();
        }

        /// <summary>
        /// Verifies that <see cref="WebParsingUtils.ProcessCsvFileAsync"/> appends a new row to
        /// <c>data.csv</c> when a row from <c>houses.csv</c> has not yet been parsed, covering the
        /// coordinate-fetch loop (lines 255–282). Coordinates may be <c>null</c> when the HTTP
        /// service is unreachable; the row is still written with the available data.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task ProcessCsvFileAsync_ShouldAppendNewRow_WhenRowIsNotYetParsed()
        {
            var tempDir = Directory.CreateTempSubdirectory().FullName;
            try
            {
                var unparsedRow = "Регіон;СтараАдмін;НоваАдмін;Громада;м.Київ Київ;col5;вул. Шевченка";
                await File.WriteAllLinesAsync(
                    Path.Combine(tempDir, "houses.csv"),
                    new[] { "header", unparsedRow },
                    Encoding.GetEncoding(1251));

                await File.WriteAllLinesAsync(
                    $"{tempDir}/data.csv",
                    new[] { "header" },
                    Encoding.GetEncoding(1251));

                var repoMock = CreateRepositoryMock();
                var utils = new WebParsingUtils(repoMock.Object);

                await utils.ProcessCsvFileAsync(tempDir);

                var lines = await File.ReadAllLinesAsync($"{tempDir}/data.csv", Encoding.GetEncoding(1251));
                lines.Should().HaveCountGreaterThan(1);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        private static string CreateTempCsvWithDataRows(params string[] dataRows)
        {
            var path = Path.GetTempFileName();
            var lines = new List<string> { "header" };
            lines.AddRange(dataRows);
            File.WriteAllLines(path, lines, Encoding.GetEncoding(1251));
            return path;
        }

        private static Mock<IRepositoryWrapper> CreateRepositoryMock()
        {
            var mock = new Mock<IRepositoryWrapper> { DefaultValue = DefaultValue.Mock };
            mock.Setup(r => r.ToponymRepository.GetAllAsync(
                    It.IsAny<Expression<Func<Toponym, bool>>>(),
                    It.IsAny<Func<IQueryable<Toponym>, IIncludableQueryable<Toponym, object>>>()))
                .ReturnsAsync(new List<Toponym>());
            mock.Setup(r => r.ToponymRepository.CreateAsync(It.IsAny<Toponym>()))
                .ReturnsAsync(new Toponym());
            mock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);
            return mock;
        }

        private static (string, string) InvokeOptimizeStreetname(string streetname)
        {
            var method = typeof(WebParsingUtils).GetMethod(
                "OptimizeStreetname",
                BindingFlags.NonPublic | BindingFlags.Static) !;
            return ((string, string))method.Invoke(null, new object[] { streetname }) !;
        }

        private static List<string> InvokeGetDistinctRows(IEnumerable<string> rows, byte beforeColumn = 7)
        {
            var method = typeof(WebParsingUtils).GetMethod(
                "GetDistinctRows",
                BindingFlags.NonPublic | BindingFlags.Static) !;
            return (List<string>)method.Invoke(null, new object[] { rows, beforeColumn }) !;
        }

        private static (string?, string?) InvokeParseJsonToCoordinateTuple(string json)
        {
            var method = typeof(WebParsingUtils).GetMethod(
                "ParseJsonToCoordinateTuple",
                BindingFlags.NonPublic | BindingFlags.Static) !;
            return ((string?, string?))method.Invoke(null, new object[] { json }) !;
        }
    }
}
