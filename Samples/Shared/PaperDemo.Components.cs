using System.Drawing;
using System.Runtime.CompilerServices;

using Prowl.PaperUI;
using Prowl.PaperUI.LayoutEngine;

namespace Shared
{
    public static class Components
    {
        public static void DefineStyles()
        {
            Button.DefineStyles();
            Input.DefineStyles();
            TextArea.DefineStyles();
            Switch.DefineStyles();
        }
    }

    public static class Button
    {
        public static void DefineStyles()
        {
            // Secondary button variant
            PaperDemo.Gui.CreateStyleFamily("shadcs-button-secondary")
                .Base(new StyleTemplate()
                    .Height(40)
                    .Rounded(8)
                    .BackgroundColor(Themes.base300)
                    .Transition(GuiProp.BackgroundColor, 0.2)
                    .Transition(GuiProp.ScaleX, 0.1)
                    .Transition(GuiProp.ScaleY, 0.1))
                .Hovered(new StyleTemplate()
                    .BackgroundColor(Color.FromArgb(150, Themes.base300)))
                .Active(new StyleTemplate()
                    .Scale(0.95)
                    .BackgroundColor(Color.FromArgb(200, Themes.primary)))
                .Register();
        }


        public static ElementBuilder Secondary(string stringID, string text = "", int intID = 0, [CallerLineNumber] int lineID = 0)
        {
            return PaperDemo.Gui.Box(stringID, intID, lineID)
                .Text(text, Fonts.arial).TextColor(Themes.baseContent).Alignment(TextAlignment.MiddleCenter)
                .Style("shadcs-button-secondary");
        }
    }

    public static class Input
    {
        public static void DefineStyles()
        {
            // Text field styles
            PaperDemo.Gui.CreateStyleFamily("shadcs-text-field-secondary")
                .Base(new StyleTemplate()
                    .Width(300)
                    .Height(40)
                    .Rounded(8)
                    .BackgroundColor(Themes.base300)
                    .BorderColor(Color.Transparent)
                    .BorderWidth(0)
                    .Transition(GuiProp.BorderColor, 0.2)
                    .Transition(GuiProp.BorderWidth, 0.2))
                .Focused(new StyleTemplate()
                    .BorderColor(Themes.primary)
                    .BorderWidth(1)
                    .BackgroundColor(Themes.base300))
                .Register();
        }

        public static ElementBuilder Secondary(string stringID, string value, Action<string> onChange = null, string placeholder = "", int intID = 0, [CallerLineNumber] int lineID = 0)
        {
            ElementBuilder parent = PaperDemo.Gui.Box(stringID, intID, lineID).Style("shadcs-text-field-secondary").TabIndex(0);
            using (parent.Enter())
            {
                PaperDemo.Gui.Box("area")
                    .Margin(8, UnitValue.StretchOne)
                    .HookToParent()
                    .IsNotInteractable()
                    .Width(UnitValue.StretchOne)
                    .Height(19)
                    .FontSize(19)
                    .TextField(value, Fonts.arial, onChange, Themes.primaryContent, placeholder, Color.FromArgb(100, Themes.primaryContent));
            }
            return parent;
        }
    }

    public static class TextArea
    {
        public static void DefineStyles()
        {
            // Text field styles
            PaperDemo.Gui.CreateStyleFamily("shadcs-text-area-secondary")
                .Base(new StyleTemplate()
                    .Width(300)
                    .Height(UnitValue.Auto)
                    //.MaxHeight(100)
                    .Rounded(8)
                    .BackgroundColor(Themes.base300)
                    .BorderColor(Color.Transparent)
                    .BorderWidth(0)
                    .Transition(GuiProp.BorderColor, 0.2)
                    .Transition(GuiProp.BorderWidth, 0.2))
                .Focused(new StyleTemplate()
                    .BorderColor(Themes.primary)
                    .BorderWidth(1)
                    .BackgroundColor(Themes.base300))
                .Register();
        }

        public static ElementBuilder Secondary(string stringID, string value, Action<string> onChange = null, string placeholder = "", int intID = 0, [CallerLineNumber] int lineID = 0)
        {
            ElementBuilder parent = PaperDemo.Gui.Box(stringID, intID, lineID).Style("shadcs-text-area-secondary").TabIndex(1);
            using (parent.Enter())
            {
                PaperDemo.Gui.Box("area")
                    .Margin(8)
                    .HookToParent()
                    .IsNotInteractable()
                    .Width(UnitValue.StretchOne)
                    .Height(UnitValue.Auto)
                    .FontSize(19)
                    .TextArea(value, Fonts.arial, onChange, placeholder, Themes.primaryContent, Color.FromArgb(100, Themes.primaryContent));
            }
            return parent;
        }
    }

    public static class Slider
    {
        public static ElementBuilder Primary(string stringID, double sliderValue, Action<double> onChange, int intID = 0, [CallerLineNumber] int lineID = 0)
        {
            var parent = PaperDemo.Gui.Box(stringID, intID, lineID)
                .Height(20)
                .Rounded(10)
                .BackgroundColor(Themes.base100)
                //.Style(BoxStyle.SolidRounded(Color.FromArgb(30, 0, 0, 0), 10f))
                .Margin(0, 0, 20, 0)
                .OnHeld((e) =>
                {
                    double parentWidth = e.ElementRect.width;
                    double pointerX = e.PointerPosition.x - e.ElementRect.x;

                    // Calculate new slider value based on pointer position
                    sliderValue = Math.Clamp(pointerX / parentWidth, 0f, 1f);
                    onChange.Invoke(sliderValue);
                });

            var cleanup = parent.Enter();

            // Filled part of slider
            using (PaperDemo.Gui.Box("SliderFill")
                .Width(PaperDemo.Gui.Percent(sliderValue * 100))
                .MinWidth(10)
                .RoundedLeft(10)
                .BackgroundColor(Themes.primary)
                //.Style(BoxStyle.SolidRounded(primaryColor, 10f))
                .Enter())
            {
                // Slider handle
                using (PaperDemo.Gui.Box("SliderHandle")
                    .Left(PaperDemo.Gui.Percent(100, -10))
                    .Width(20)
                    .Height(20)
                    .Rounded(10)
                    .BackgroundColor(Themes.primaryContent)
                    //.Style(BoxStyle.SolidRounded(textColor, 10f))
                    .PositionType(PositionType.SelfDirected)
                    .Enter()) { }
            }

            cleanup.Dispose();

            return parent;
        }

        public static ElementBuilder Secondary(string stringID, double sliderValue, Action<double> onChange, int intID = 0, [CallerLineNumber] int lineID = 0)
        {
            var parent = PaperDemo.Gui.Box(stringID, intID, lineID)
                .Height(20)
                .Rounded(10)
                .BackgroundColor(Themes.base100)
                //.Style(BoxStyle.SolidRounded(Color.FromArgb(30, 0, 0, 0), 10f))
                .Margin(0, 0, 20, 0)
                .OnHeld((e) =>
                {
                    double parentWidth = e.ElementRect.width;
                    double pointerX = e.PointerPosition.x - e.ElementRect.x;

                    // Calculate new slider value based on pointer position
                    sliderValue = Math.Clamp(pointerX / parentWidth, 0f, 1f);
                    onChange.Invoke(sliderValue);
                });

            var cleanup = parent.Enter();

            // Filled part of slider
            using (PaperDemo.Gui.Box("SliderFill")
                .Width(PaperDemo.Gui.Percent(sliderValue * 100))
                .MinWidth(10)
                .RoundedLeft(10)
                .BackgroundColor(Themes.base300)
                //.Style(BoxStyle.SolidRounded(primaryColor, 10f))
                .Enter())
            {
                // Slider handle
                using (PaperDemo.Gui.Box("SliderHandle")
                    .Left(PaperDemo.Gui.Percent(100, -10))
                    .Width(20)
                    .Height(20)
                    .Rounded(10)
                    .BackgroundColor(Themes.primaryContent)
                    //.Style(BoxStyle.SolidRounded(textColor, 10f))
                    .PositionType(PositionType.SelfDirected)
                    .Enter()) { }
            }

            cleanup.Dispose();

            return parent;
        }
    }

    public static class Switch
    {
        public static void DefineStyles()
        {

            // Toggle switch styles
            PaperDemo.Gui.CreateStyleFamily("toggle")
                .Base(new StyleTemplate()
                    .Width(60)
                    .Height(30)
                    .Rounded(20)
                    .Transition(GuiProp.BackgroundColor, 0.25, Easing.CubicInOut))
                .Register();

            PaperDemo.Gui.RegisterStyle("toggle-on", new StyleTemplate()
                .BackgroundColor(Themes.primary));

            PaperDemo.Gui.RegisterStyle("toggle-off", new StyleTemplate()
                .BackgroundColor(Color.FromArgb(100, Themes.base300)));

            PaperDemo.Gui.RegisterStyle("toggle-dot", new StyleTemplate()
                .Width(24)
                .Height(24)
                .Rounded(20)
                .BackgroundColor(Color.White)
                //.PositionType(PositionType.SelfDirected)
                .Top(PaperDemo.Gui.Pixels(3))
                .Transition(GuiProp.Left, 0.25, Easing.CubicInOut));
        }

        public static ElementBuilder Primary(string stringID, bool isOn, int intID = 0, [CallerLineNumber] int lineID = 0)
        {
            // Toggle switch - much simpler with styles!
            // bool isOn = toggleState[i];
            // int index = i;

            ElementBuilder builder;
            using ((builder = PaperDemo.Gui.Box(stringID, intID, lineID)
                .Style("toggle")
                .StyleIf(isOn, "toggle-on")
                .StyleIf(!isOn, "toggle-off")).Enter())
            {
                PaperDemo.Gui.Box("ToggleDot")
                    .Style("toggle-dot")
                    .Left(PaperDemo.Gui.Pixels(isOn ? 32 : 4));
            }
            return builder;
        }
    }

    public static class PieChart
    {
        public static ElementBuilder Primary(string stringID, double[] values, double startAngle, int intID = 0, [CallerLineNumber] int lineID = 0)
        {
            // "Analysis" mock content
            ElementBuilder builder;
            using ((builder = PaperDemo.Gui.Box(stringID, intID, lineID)
                .Margin(20)).Enter())
            {

                // Add a simple pie chart visualization
                PaperDemo.Gui.AddActionElement((vg, rect) =>
                {
                    double centerX = rect.x + rect.width / 2;
                    double centerY = rect.y + rect.height / 2;
                    double radius = Math.Min(rect.width, rect.height) * 0.4f;

                    // double startAngle = 0;
                    // double[] values = { sliderValue, 0.2f, 0.15f, 0.25f, 0.1f };

                    // Normalize Values
                    double total = values.Sum();
                    for (int i = 0; i < values.Length; i++)
                        values[i] /= total;


                    for (int i = 0; i < values.Length; i++)
                    {
                        // Calculate angles
                        double angle = values[i] * Math.PI * 2;
                        double endAngle = startAngle + angle;

                        // Draw pie slice
                        vg.BeginPath();
                        vg.MoveTo(centerX, centerY);
                        vg.Arc(centerX, centerY, radius, startAngle, endAngle);
                        vg.LineTo(centerX, centerY);
                        vg.SetFillColor(Themes.colorPalette[i % Themes.colorPalette.Length]);
                        vg.Fill();

                        // Draw outline
                        vg.BeginPath();
                        vg.MoveTo(centerX, centerY);
                        vg.Arc(centerX, centerY, radius, startAngle, endAngle);
                        vg.LineTo(centerX, centerY);
                        vg.SetStrokeColor(Color.White);
                        vg.SetStrokeWidth(2);
                        vg.Stroke();

                        // Draw percentage labels
                        double labelAngle = startAngle + angle / 2;
                        double labelRadius = radius * 0.7f;
                        double labelX = centerX + Math.Cos(labelAngle) * labelRadius;
                        double labelY = centerY + Math.Sin(labelAngle) * labelRadius;

                        string label = $"{values[i] * 100:F0}%";
                        vg.SetFillColor(Color.White);
                        //vg.TextAlign(Align.Center | Align.Middle);
                        //vg.FontSize(16);
                        //vg.Text(labelX, labelY, label);
                        vg.DrawText(label, labelX, labelY, Color.White, 18, Fonts.arial);

                        // Move to next slice
                        startAngle = endAngle;
                    }

                    // Draw center circle
                    vg.BeginPath();
                    vg.Circle(centerX, centerY, radius * 0.4f);
                    vg.SetFillColor(Color.White);
                    vg.Fill();
                });
            }
            return builder;
        }
    }
}