using Prowl.PaperUI;

using Prowl.Vector;

namespace Shared.Tabs
{
    public class GameTab : Tab
    {
        double time = 0;
        static double[] dataPoints = { 0.2f, 0.5f, 0.3f, 0.8f, 0.4f, 0.7f, 0.6f };
        static Vector2 chartPosition = new Vector2(0, 0);
        static double zoomLevel = 1.0f;

        public GameTab(Paper gui) : base(gui)
        {
            title = "Game";
            id = "game";
            width = 55;
        }

        public override void Body()
        {
            // Update time for animations
            // time += 0.016f; // Assuming ~60fps
            time += Gui.DeltaTime;

            using (Gui.Row("Tools").RowBetween(5).Margin(4).Top(5).Height(20).Enter())
            {
                Gui.Box("Tool 1")
                    .Text(Icons.ArrowsTurnToDots, Fonts.arial).FontSize(12)
                    .Width(20).Rounded(5).Alignment(TextAlignment.MiddleCenter)
                    .Hovered.BackgroundColor(Themes.base250).End();
                Gui.Box("Tool 2")
                    .Text(Icons.ArrowsDownToLine, Fonts.arial).FontSize(12)
                    .Width(20).Rounded(5).Alignment(TextAlignment.MiddleCenter)
                    .Hovered.BackgroundColor(Themes.base250).End();
                Gui.Box("Tool 3")
                    .Text(Icons.Anchor, Fonts.arial).FontSize(12)
                    .Width(20).Rounded(5).Alignment(TextAlignment.MiddleCenter)
                    .Hovered.BackgroundColor(Themes.base250).End();
                Gui.Box("Tool 4")
                    .Text(Icons.AngleUp, Fonts.arial).FontSize(12)
                    .Width(20).Rounded(5).Alignment(TextAlignment.MiddleCenter)
                    .Hovered.BackgroundColor(Themes.base250).End();

                Gui.Box("Spacer");

                Gui.Box("Tool 4")
                .Text(Icons.ArrowsLeftRight, Fonts.arial).FontSize(12)
                .Width(20).Rounded(5).Alignment(TextAlignment.MiddleCenter)
                .Hovered.BackgroundColor(Themes.base250).End();
            }

            using (Gui.Box("Game View").Margin(2).Rounded(5).BackgroundColor(System.Drawing.Color.Black).Enter())
            {

                // Chart content
                using (Gui.Box("Chart")
                    .Margin(20)
                    .OnDragging((e) => chartPosition += e.Delta)
                    .OnScroll((e) => zoomLevel = Math.Clamp(zoomLevel + e.Delta * 0.1f, 0.5f, 2.0f))
                    .Clip()
                    .Enter())
                {
                    using (Gui.Box("ChartCanvas")
                        .Translate(chartPosition.x, chartPosition.y)
                        .Scale(zoomLevel)
                        .Enter())
                    {
                        // Draw a simple chart with animated data
                        Gui.AddActionElement((vg, rect) =>
                        {

                            // Draw grid lines
                            for (int i = 0; i <= 5; i++)
                            {
                                double y = rect.y + (rect.height / 5) * i;
                                vg.BeginPath();
                                vg.MoveTo(rect.x, y);
                                vg.LineTo(rect.x + rect.width, y);
                                vg.SetStrokeColor(Themes.baseContent);
                                vg.SetStrokeWidth(1);
                                vg.Stroke();
                            }

                            // Draw animated data points
                            vg.BeginPath();
                            double pointSpacing = rect.width / (dataPoints.Length - 1);
                            double animatedValue;

                            // Draw fill
                            vg.MoveTo(rect.x, rect.y + rect.height);

                            for (int i = 0; i < dataPoints.Length; i++)
                            {
                                animatedValue = dataPoints[i] + Math.Sin(time * 0.25f + i * 0.5f) * 0.1f;
                                //animatedValue = Math.Clamp(animatedValue, 0.1f, 0.9f);
                                animatedValue = Math.Min(Math.Max(animatedValue, 0.1f), 0.9f); // Clamp to [0.1, 0.9]

                                double x = rect.x + i * pointSpacing;
                                double y = rect.y + rect.height - (animatedValue * rect.height);

                                if (i == 0)
                                    vg.MoveTo(x, y);
                                else
                                    vg.LineTo(x, y);
                            }

                            // Complete the fill path
                            vg.LineTo(rect.x + rect.width, rect.y + rect.height);
                            vg.LineTo(rect.x, rect.y + rect.height);

                            // Fill with gradient
                            //var paint = vg.LinearGradient(
                            //    rect.x, rect.y,
                            //    rect.x, rect.y + rect.height,
                            //    Color.FromArgb(100, primaryColor),
                            //    Color.FromArgb(10, primaryColor));
                            //vg.SetFillPaint(paint);
                            vg.SaveState();
                            vg.SetLinearBrush(rect.x, rect.y, rect.x, rect.y + rect.height, System.Drawing.Color.FromArgb(100, Themes.primary), System.Drawing.Color.FromArgb(10, Themes.primary));
                            vg.FillComplex();
                            vg.RestoreState();

                            vg.ClosePath();

                            // Draw the line
                            vg.BeginPath();
                            for (int i = 0; i < dataPoints.Length; i++)
                            {
                                animatedValue = dataPoints[i] + Math.Sin(time * 0.25f + i * 0.5f) * 0.1f;
                                //animatedValue = Math.Clamp(animatedValue, 0.1f, 0.9f);
                                animatedValue = Math.Min(Math.Max(animatedValue, 0.1f), 0.9f); // Clamp to [0.1, 0.9]

                                double x = rect.x + i * pointSpacing;
                                double y = rect.y + rect.height - (animatedValue * rect.height);

                                if (i == 0)
                                    vg.MoveTo(x, y);
                                else
                                    vg.LineTo(x, y);
                            }

                            vg.SetStrokeColor(Themes.primary);
                            vg.SetStrokeWidth(3);
                            vg.Stroke();

                            // Draw points
                            for (int i = 0; i < dataPoints.Length; i++)
                            {
                                animatedValue = dataPoints[i] + Math.Sin(time * 0.25f + i * 0.5f) * 0.1f;
                                //animatedValue = Math.Clamp(animatedValue, 0.1f, 0.9f);
                                animatedValue = Math.Min(Math.Max(animatedValue, 0.1f), 0.9f); // Clamp to [0.1, 0.9]

                                double x = rect.x + i * pointSpacing;
                                double y = rect.y + rect.height - (animatedValue * rect.height);

                                vg.BeginPath();
                                vg.Circle(x, y, 6);
                                vg.SetFillColor(System.Drawing.Color.White);
                                vg.Fill();

                                vg.BeginPath();
                                vg.Circle(x, y, 4);
                                vg.SetFillColor(Themes.primary);
                                vg.Fill();
                            }

                            vg.ClosePath();
                        });
                    }
                }


                using (Gui.Row("Details").Height(28).Margin(8).Enter())
                {
                    // FPS Counter
                    Gui.Box("FPS").Text($"FPS: {1f / Gui.DeltaTime:F1}", Fonts.arial).TextColor(Themes.baseContent).Alignment(TextAlignment.MiddleLeft).FontSize(19);
                    Gui.Box("NodeCounter").Text($"Nodes: {Gui.CountOfAllElements}", Fonts.arial).TextColor(Themes.baseContent).Alignment(TextAlignment.MiddleLeft).FontSize(19);
                    Gui.Box("MS").Text($"Frame ms: {Gui.MillisecondsSpent}", Fonts.arial).TextColor(Themes.baseContent).Alignment(TextAlignment.MiddleLeft).FontSize(19);
                }
            }
        }
    }
}