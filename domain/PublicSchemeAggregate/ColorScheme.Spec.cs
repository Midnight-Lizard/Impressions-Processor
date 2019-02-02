using FluentAssertions;
using FluentValidation.TestHelper;
using MidnightLizard.Testing.Utilities;
using System.Linq;
using Xunit;

namespace MidnightLizard.Impressions.Domain.PublicSchemeAggregate
{
    public class ColorSchemeSpec : ColorScheme
    {
        public static ColorScheme CorrectColorScheme => new ColorScheme
        {
            colorSchemeId = "almondRipe",
            colorSchemeName = "Almond Ripe",
            runOnThisSite = true,
            blueFilter = 5,
            useDefaultSchedule = true,
            scheduleStartHour = 0,
            scheduleFinishHour = 24,
            backgroundSaturationLimit = 80,
            backgroundContrast = 50,
            backgroundLightnessLimit = 11,
            backgroundGraySaturation = 30,
            backgroundGrayHue = 36,
            textSaturationLimit = 90,
            textContrast = 60,
            textLightnessLimit = 80,
            textGraySaturation = 10,
            textGrayHue = 88,
            textSelectionHue = 36,
            linkSaturationLimit = 80,
            linkContrast = 50,
            linkLightnessLimit = 70,
            linkDefaultSaturation = 60,
            linkDefaultHue = 88,
            linkVisitedHue = 122,
            borderSaturationLimit = 80,
            borderContrast = 30,
            borderLightnessLimit = 50,
            borderGraySaturation = 20,
            borderGrayHue = 54,
            imageLightnessLimit = 80,
            imageSaturationLimit = 90,
            useImageHoverAnimation = false,
            backgroundImageLightnessLimit = 40,
            backgroundImageSaturationLimit = 80,
            scrollbarSaturationLimit = 20,
            scrollbarContrast = 0,
            scrollbarLightnessLimit = 40,
            scrollbarGrayHue = 45,
            scrollbarSize = 10,

            backgroundReplaceAllHues = false,
            borderReplaceAllHues = false,
            buttonReplaceAllHues = false,
            linkReplaceAllHues = true,
            textReplaceAllHues = false,

            buttonContrast = 50,
            buttonGrayHue = 40,
            buttonGraySaturation = 30,
            buttonLightnessLimit = 30,
            buttonSaturationLimit = 90,

            // v9.3
            doNotInvertContent = false,
            mode = ColorSchemeMode.Auto,
            modeAutoSwitchLimit = 5000,
            includeMatches = "http://test.com/*",
            excludeMatches = "https://exclude.this/*",
            backgroundHueGravity = 0,
            buttonHueGravity = 0,
            textHueGravity = 0,
            linkHueGravity = 80,
            borderHueGravity = 0,
            scrollbarStyle = "true",

            // v10.1
            hideBigBackgroundImages = true,
            maxBackgroundImageSize = 123,
        };

        public class GetPropertyValuesSpec : ColorSchemeSpec
        {
            [It(nameof(GetPropertyValues))]
            public void Should_return_values_of_all_properties()
            {
                this.GetPropertyValues().Count().Should().Be(typeof(ColorScheme).GetProperties().Length);
            }
        }

        public class ValidatorSpec : ColorSchemeSpec
        {
            private readonly ColorSchemeValidator validator = new ColorSchemeValidator();

            [It(nameof(ColorSchemeValidator))]
            public void Should_fail_when_colorSchemeName_is_null()
            {
                this.validator.ShouldHaveValidationErrorFor(cs => cs.colorSchemeName, null as string);
            }

            [It(nameof(ColorSchemeValidator))]
            public void Should_fail_when_colorSchemeId_is_null()
            {
                this.validator.ShouldHaveValidationErrorFor(cs => cs.colorSchemeId, null as string);
            }

            [It(nameof(ColorSchemeValidator))]
            public void Should_fail_when_colorSchemeName_is_empty()
            {
                this.validator.ShouldHaveValidationErrorFor(cs => cs.colorSchemeName, string.Empty);
            }

            [It(nameof(ColorSchemeValidator))]
            public void Should_fail_when_scrollbarStyle_is_empty()
            {
                this.validator.ShouldHaveValidationErrorFor(cs => cs.scrollbarStyle, string.Empty);
            }

            [It(nameof(ColorSchemeValidator))]
            public void Should_fail_when_colorSchemeId_is_empty()
            {
                this.validator.ShouldHaveValidationErrorFor(cs => cs.colorSchemeId, string.Empty);
            }

            [It(nameof(ColorSchemeValidator))]
            public void Should_fail_when_colorSchemeName_is_too_long()
            {
                this.validator.ShouldHaveValidationErrorFor(cs => cs.colorSchemeName, new string('*', 51));
            }

            [InlineData("simple"), InlineData("modern")]
            [Its(nameof(ColorSchemeValidator))]
            public void Should_fail_when_scrollbarStyle_is_out_of_range(string value)
            {
                this.validator.ShouldHaveValidationErrorFor(cs => cs.scrollbarStyle, value);
            }

            [InlineData("true"), InlineData("false")]
            [Its(nameof(ColorSchemeValidator))]
            public void Should_succeed_when_scrollbarStyle_is_in_range(string value)
            {
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.scrollbarStyle, value);
            }

            [InlineData((ColorSchemeMode)5), InlineData((ColorSchemeMode)10)]
            [Its(nameof(ColorSchemeValidator))]
            public void Should_fail_when_mode_is_out_of_range(ColorSchemeMode value)
            {
                this.validator.ShouldHaveValidationErrorFor(cs => cs.mode, value);
            }

            [InlineData(ColorSchemeMode.Auto), InlineData(ColorSchemeMode.Complex), InlineData(ColorSchemeMode.Simple)]
            [Its(nameof(ColorSchemeValidator))]
            public void Should_succeed_when_mode_is_in_range(ColorSchemeMode value)
            {
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.mode, value);
            }

            [It(nameof(ColorSchemeValidator))]
            public void Should_fail_when_colorSchemeId_is_too_long()
            {
                this.validator.ShouldHaveValidationErrorFor(cs => cs.colorSchemeId, new string('*', 51));
            }

            [InlineData(360), InlineData(361), InlineData(400), InlineData(999)]
            [Its(nameof(ColorSchemeValidator))]
            public void Should_fail_when_Hue_is_out_of_range(int value)
            {
                this.validator.ShouldHaveValidationErrorFor(cs => cs.backgroundGrayHue, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.borderGrayHue, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.buttonGrayHue, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.linkDefaultHue, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.linkVisitedHue, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.scrollbarGrayHue, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.textGrayHue, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.textSelectionHue, value);
            }

            [InlineData(0), InlineData(180), InlineData(300), InlineData(259)]
            [Its(nameof(ColorSchemeValidator))]
            public void Should_succeed_when_Hue_is_in_0____359_range(int value)
            {
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.backgroundGrayHue, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.borderGrayHue, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.buttonGrayHue, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.linkDefaultHue, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.linkVisitedHue, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.scrollbarGrayHue, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.textGrayHue, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.textSelectionHue, value);
            }

            [InlineData(10001), InlineData(20000)]
            [Its(nameof(ColorSchemeValidator))]
            public void Should_fail_when_modeAutoSwitchLimit_is_out_of_range(int value)
            {
                this.validator.ShouldHaveValidationErrorFor(cs => cs.modeAutoSwitchLimit, value);
            }

            [InlineData(0), InlineData(1500), InlineData(3500), InlineData(6500), InlineData(9999), InlineData(10000)]
            [Its(nameof(ColorSchemeValidator))]
            public void Should_succeed_when_modeAutoSwitchLimit_is_in_0____10000_range(int value)
            {
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.modeAutoSwitchLimit, value);
            }

            [It(nameof(ColorSchemeValidator))]
            public void Should_fail_when_any_setting_is_negative()
            {
                this.validator.ShouldHaveValidationErrorFor(cs => cs.blueFilter, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.scheduleStartHour, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.scheduleFinishHour, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.backgroundSaturationLimit, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.backgroundContrast, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.backgroundLightnessLimit, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.backgroundGraySaturation, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.backgroundGrayHue, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.textSaturationLimit, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.textContrast, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.textLightnessLimit, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.textGraySaturation, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.textGrayHue, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.textSelectionHue, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.linkSaturationLimit, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.linkContrast, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.linkLightnessLimit, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.linkDefaultSaturation, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.linkDefaultHue, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.linkVisitedHue, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.borderSaturationLimit, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.borderContrast, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.borderLightnessLimit, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.borderGraySaturation, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.borderGrayHue, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.imageLightnessLimit, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.imageSaturationLimit, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.backgroundImageLightnessLimit, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.backgroundImageSaturationLimit, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.scrollbarSaturationLimit, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.scrollbarContrast, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.scrollbarLightnessLimit, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.scrollbarGrayHue, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.buttonSaturationLimit, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.buttonContrast, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.buttonLightnessLimit, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.buttonGraySaturation, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.buttonGrayHue, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.scrollbarSize, -1);
                // v9.3
                this.validator.ShouldHaveValidationErrorFor(cs => cs.backgroundHueGravity, -1);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.borderHueGravity, -2);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.buttonHueGravity, -3);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.linkHueGravity, -4);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.textHueGravity, -5);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.modeAutoSwitchLimit, -6);
                // v 10.1
                this.validator.ShouldHaveValidationErrorFor(cs => cs.maxBackgroundImageSize, -2);
            }

            [InlineData(101), InlineData(110), InlineData(200), InlineData(999)]
            [Its(nameof(ColorSchemeValidator))]
            public void Should_fail_when_any_percentage_is_greater_than_100(int value)
            {
                this.validator.ShouldHaveValidationErrorFor(cs => cs.blueFilter, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.backgroundSaturationLimit, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.backgroundContrast, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.backgroundLightnessLimit, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.backgroundGraySaturation, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.textSaturationLimit, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.textContrast, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.textLightnessLimit, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.textGraySaturation, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.linkSaturationLimit, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.linkContrast, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.linkLightnessLimit, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.linkDefaultSaturation, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.borderSaturationLimit, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.borderContrast, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.borderLightnessLimit, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.borderGraySaturation, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.imageLightnessLimit, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.imageSaturationLimit, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.backgroundImageLightnessLimit, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.backgroundImageSaturationLimit, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.scrollbarSaturationLimit, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.scrollbarContrast, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.scrollbarLightnessLimit, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.buttonSaturationLimit, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.buttonContrast, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.buttonLightnessLimit, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.buttonGraySaturation, value);
                // v9.3
                this.validator.ShouldHaveValidationErrorFor(cs => cs.backgroundHueGravity, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.borderHueGravity, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.buttonHueGravity, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.linkHueGravity, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.textHueGravity, value);
            }

            [InlineData(0), InlineData(30), InlineData(60), InlineData(100)]
            [Its(nameof(ColorSchemeValidator))]
            public void Should_succeed_when_all_percentages_are_in_0____100_range(int value)
            {
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.blueFilter, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.backgroundSaturationLimit, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.backgroundContrast, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.backgroundLightnessLimit, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.backgroundGraySaturation, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.textSaturationLimit, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.textContrast, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.textLightnessLimit, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.textGraySaturation, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.linkSaturationLimit, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.linkContrast, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.linkLightnessLimit, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.linkDefaultSaturation, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.borderSaturationLimit, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.borderContrast, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.borderLightnessLimit, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.borderGraySaturation, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.imageLightnessLimit, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.imageSaturationLimit, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.backgroundImageLightnessLimit, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.backgroundImageSaturationLimit, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.scrollbarSaturationLimit, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.scrollbarContrast, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.scrollbarLightnessLimit, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.buttonSaturationLimit, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.buttonContrast, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.buttonLightnessLimit, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.buttonGraySaturation, value);
                // v9.3
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.backgroundHueGravity, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.borderHueGravity, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.buttonHueGravity, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.linkHueGravity, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.textHueGravity, value);
            }

            [InlineData(25), InlineData(36), InlineData(48), InlineData(72)]
            [Its(nameof(ColorSchemeValidator))]
            public void Should_fail_when_Hours_exceed_24(int value)
            {
                this.validator.ShouldHaveValidationErrorFor(cs => cs.scheduleStartHour, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.scheduleFinishHour, value);
            }

            [InlineData(0), InlineData(8), InlineData(12), InlineData(15), InlineData(24)]
            [Its(nameof(ColorSchemeValidator))]
            public void Should_succeeed_when_Hours_are_in_0____24_range(int value)
            {
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.scheduleStartHour, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.scheduleFinishHour, value);
            }

            [InlineData(51), InlineData(99), InlineData(100)]
            [Its(nameof(ColorSchemeValidator))]
            public void Should_fail_when_Scrollbar__Size_exceeds_50px(int px)
            {
                this.validator.ShouldHaveValidationErrorFor(cs => cs.scrollbarSize, px);
            }

            [InlineData(0), InlineData(5), InlineData(20), InlineData(50)]
            [Its(nameof(ColorSchemeValidator))]
            public void Should_succeeed_when_Scrollbar__Size_is_in_0____50px_range(int px)
            {
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.scrollbarSize, px);
            }

            [It(nameof(ColorSchemeValidator))]
            public void Should_fail_with_default_ColorScheme_object()
            {
                this.validator.ShouldHaveValidationErrorFor(cs => cs.colorSchemeName, new ColorScheme());
            }

            [InlineData(".*"), InlineData(".+"), InlineData(".?")]
            [InlineData("sftp://test.com/*")]
            [InlineData("*://test.*/*")]
            [InlineData("http://test.com")]
            [InlineData("http:/test.com/")]
            [InlineData("file://C:/test.html")]
            [InlineData("file://x/C:/test.html")]
            [InlineData("test.com/*")]
            [Its(nameof(ColorSchemeValidator))]
            public void Should_fail_with_incorrect_match_patterns(string value)
            {
                this.validator.ShouldHaveValidationErrorFor(cs => cs.includeMatches, value);
                this.validator.ShouldHaveValidationErrorFor(cs => cs.excludeMatches, value);
            }

            [InlineData("*"), InlineData("*://*/*"), InlineData("<all_urls>")]
            [InlineData("ftp://test.com/*")]
            [InlineData("http://test.com/*")]
            [InlineData("http://test.com/")]
            [InlineData("HTTP://TEST.COM/TEST")]
            [InlineData("https://test.com/*?param=2")]
            [InlineData("file:///C:/test.html")]
            [InlineData("*://*.test.com/*")]
            [InlineData("*://*.test.com/*test*")]
            [Its(nameof(ColorSchemeValidator))]
            public void Should_succeed_with_correct_match_patterns(string value)
            {
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.includeMatches, value);
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.excludeMatches, value);
            }

            // v 10.1
            [InlineData(0), InlineData(100), InlineData(650), InlineData(1000)]
            [Its(nameof(ColorSchemeValidator))]
            public void Should_succeed_when_maxBackgroundImageSize_is_in_0____1000_range(int value)
            {
                this.validator.ShouldNotHaveValidationErrorFor(cs => cs.maxBackgroundImageSize, value);
            }

            // v 10.1
            [InlineData(1001), InlineData(2000)]
            [Its(nameof(ColorSchemeValidator))]
            public void Should_fail_when_maxBackgroundImageSize_is_out_of_range(int value)
            {
                this.validator.ShouldHaveValidationErrorFor(cs => cs.maxBackgroundImageSize, value);
            }
        }
    }
}
