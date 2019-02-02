using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MidnightLizard.Impressions.Domain.PublicSchemeAggregate
{
    public class ColorSchemeValidator : AbstractValidator<ColorScheme>
    {
        public ColorSchemeValidator()
        {
            RuleFor(cs => cs.colorSchemeId).NotEmpty().Length(1, 50);
            RuleFor(cs => cs.colorSchemeName).NotEmpty().Length(1, 50);

            RuleFor(cs => cs.scheduleFinishHour).InclusiveBetween(0, 24);
            RuleFor(cs => cs.scheduleStartHour).InclusiveBetween(0, 24);

            RuleFor(cs => cs.blueFilter).InclusiveBetween(0, 100);
            RuleFor(cs => cs.backgroundSaturationLimit).InclusiveBetween(0, 100);
            RuleFor(cs => cs.backgroundContrast).InclusiveBetween(0, 100);
            RuleFor(cs => cs.backgroundLightnessLimit).InclusiveBetween(0, 100);
            RuleFor(cs => cs.backgroundGraySaturation).InclusiveBetween(0, 100);
            RuleFor(cs => cs.backgroundGrayHue).InclusiveBetween(0, 359);
            RuleFor(cs => cs.textSaturationLimit).InclusiveBetween(0, 100);
            RuleFor(cs => cs.textContrast).InclusiveBetween(0, 100);
            RuleFor(cs => cs.textLightnessLimit).InclusiveBetween(0, 100);
            RuleFor(cs => cs.textGraySaturation).InclusiveBetween(0, 100);
            RuleFor(cs => cs.textGrayHue).InclusiveBetween(0, 359);
            RuleFor(cs => cs.textSelectionHue).InclusiveBetween(0, 359);
            RuleFor(cs => cs.linkSaturationLimit).InclusiveBetween(0, 100);
            RuleFor(cs => cs.linkContrast).InclusiveBetween(0, 100);
            RuleFor(cs => cs.linkLightnessLimit).InclusiveBetween(0, 100);
            RuleFor(cs => cs.linkDefaultSaturation).InclusiveBetween(0, 100);
            RuleFor(cs => cs.linkDefaultHue).InclusiveBetween(0, 359);
            RuleFor(cs => cs.linkVisitedHue).InclusiveBetween(0, 359);
            RuleFor(cs => cs.borderSaturationLimit).InclusiveBetween(0, 100);
            RuleFor(cs => cs.borderContrast).InclusiveBetween(0, 100);
            RuleFor(cs => cs.borderLightnessLimit).InclusiveBetween(0, 100);
            RuleFor(cs => cs.borderGraySaturation).InclusiveBetween(0, 100);
            RuleFor(cs => cs.borderGrayHue).InclusiveBetween(0, 359);
            RuleFor(cs => cs.imageLightnessLimit).InclusiveBetween(0, 100);
            RuleFor(cs => cs.imageSaturationLimit).InclusiveBetween(0, 100);
            RuleFor(cs => cs.backgroundImageLightnessLimit).InclusiveBetween(0, 100);
            RuleFor(cs => cs.backgroundImageSaturationLimit).InclusiveBetween(0, 100);
            RuleFor(cs => cs.scrollbarSaturationLimit).InclusiveBetween(0, 100);
            RuleFor(cs => cs.scrollbarContrast).InclusiveBetween(0, 100);
            RuleFor(cs => cs.scrollbarLightnessLimit).InclusiveBetween(0, 100);
            RuleFor(cs => cs.scrollbarGrayHue).InclusiveBetween(0, 359);
            RuleFor(cs => cs.buttonSaturationLimit).InclusiveBetween(0, 100);
            RuleFor(cs => cs.buttonContrast).InclusiveBetween(0, 100);
            RuleFor(cs => cs.buttonLightnessLimit).InclusiveBetween(0, 100);
            RuleFor(cs => cs.buttonGraySaturation).InclusiveBetween(0, 100);
            RuleFor(cs => cs.buttonGrayHue).InclusiveBetween(0, 359);
            RuleFor(cs => cs.scrollbarSize).InclusiveBetween(0, 50);

            // v9.3
            string schemeSegment = @"(\*|http|https|ftp)",
              hostSegment = @"(\*|(?:\*\.)?(?:[^/*]+))",
              fileScheme = @"file://",
              pathSegment = @"(.*)";
            var regExp = new Regex($@"^$|^\*$|^<all_urls>$|^(?:{fileScheme}|{schemeSegment}://{hostSegment})/{pathSegment}$", RegexOptions.IgnoreCase);

            RuleFor(cs => cs.includeMatches).Matches(regExp);
            RuleFor(cs => cs.excludeMatches).Matches(regExp);
            RuleFor(cs => cs.includeMatches).Length(0, 4000);
            RuleFor(cs => cs.excludeMatches).Length(0, 4000);

            RuleFor(cs => cs.backgroundHueGravity).InclusiveBetween(0, 100);
            RuleFor(cs => cs.buttonHueGravity).InclusiveBetween(0, 100);
            RuleFor(cs => cs.textHueGravity).InclusiveBetween(0, 100);
            RuleFor(cs => cs.linkHueGravity).InclusiveBetween(0, 100);
            RuleFor(cs => cs.borderHueGravity).InclusiveBetween(0, 100);
            RuleFor(cs => cs.scrollbarStyle).NotEmpty().Matches("^(?:true|false)$");
            RuleFor(cs => cs.modeAutoSwitchLimit).InclusiveBetween(0, 10000);
            RuleFor(cs => cs.mode).IsInEnum();

            // v 10.1
            RuleFor(cs => cs.maxBackgroundImageSize).InclusiveBetween(0, 1000);
        }
    }
}
