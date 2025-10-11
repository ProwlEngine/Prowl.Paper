using Prowl.PaperUI;
using Prowl.Vector;
using Prowl.Scribe;
using Prowl.Quill;

namespace Shared.Tabs
{
    public class SlimeFriendsTab : Tab
    {
        const int SLIME_SIZE = 30;
        const double BASE_SLIME_SPEED = 80.0;
        const double HOP_DURATION = 1.0;
        const double AVOIDANCE_RADIUS = 60.0;
        const double AVOIDANCE_STRENGTH = 200.0;
        const double HEART_DISPLAY_DURATION = 2.0;
        const double SPAWN_BUTTON_WIDTH = 120;
        const double SPAWN_BUTTON_HEIGHT = 40;
        const double SPAWN_BUTTON_X = 10;
        const double SPAWN_BUTTON_Y = 10;

        struct Slime
        {
            public Vector2 position;
            public System.Drawing.Color color;
            public string name;
            public double hopProgress;
            public double speed;
            public double heartTimer;

            public Slime(Vector2 position, System.Drawing.Color color, string name, double speed)
            {
                this.position = position;
                this.color = color;
                this.name = name;
                this.hopProgress = 0;
                this.speed = speed;
                this.heartTimer = 0;
            }
        }

        struct GrassBlade
        {
            public double x;
            public double y;
            public double height;
        }

        struct Flower
        {
            public double x;
            public double y;
            public System.Drawing.Color color;
        }

        List<Slime> slimes = new List<Slime>();
        List<GrassBlade> grassBlades = new List<GrassBlade>();
        List<Flower> flowers = new List<Flower>();
        Vector2 pointerPosition = Vector2.zero;
        
        TextLayoutSettings nameLayout = new TextLayoutSettings
        {
            Font = Fonts.arial,
            PixelSize = 12,
            Alignment = Prowl.Scribe.TextAlignment.Center
        };

        TextLayoutSettings buttonLayout = new TextLayoutSettings
        {
            Font = Fonts.arial,
            PixelSize = 14,
            Alignment = Prowl.Scribe.TextAlignment.Center
        };

        string[] slimeNames = { "Blob", "Gooey", "Squishy", "Bounce", "Jelly", "Puddle", "Glop", "Wobble", "Splat", "Mochi" };
        int nameIndex = 0;

        public SlimeFriendsTab(Paper gui) : base(gui)
        {
            title = "Slime Friends";
            id = "slimefriends";
            width = 110;

            // Initialize background elements
            System.Random rand = new System.Random(42);
            System.Drawing.Color[] flowerColors = {
                System.Drawing.Color.FromArgb(255, 182, 193),
                System.Drawing.Color.FromArgb(255, 255, 102),
                System.Drawing.Color.FromArgb(186, 143, 255)
            };

            // Generate grass blades (relative positions 0-1)
            for (int i = 0; i < 30; i++)
            {
                grassBlades.Add(new GrassBlade
                {
                    x = rand.NextDouble(),
                    y = 0.6 + rand.NextDouble() * 0.4,
                    height = 10 + rand.NextDouble() * 15
                });
            }

            // Generate flowers (relative positions 0-1)
            for (int i = 0; i < 10; i++)
            {
                flowers.Add(new Flower
                {
                    x = rand.NextDouble(),
                    y = 0.65 + rand.NextDouble() * 0.3,
                    color = flowerColors[rand.Next(flowerColors.Length)]
                });
            }

            // Start with only 1 slime
            rand = new System.Random();
            slimes.Add(new Slime(
                new Vector2(250, 200),
                System.Drawing.Color.FromArgb(
                    rand.Next(100, 255),
                    rand.Next(100, 255),
                    rand.Next(100, 255)
                ),
                slimeNames[nameIndex++],
                BASE_SLIME_SPEED * (0.7 + rand.NextDouble() * 0.6)
            ));
        }

        private double CircleSDF(Vector2 point, Vector2 center, double radius)
        {
            return (point - center).magnitude - radius;
        }

        private void DrawPrairieBackground(Canvas canvas, Rect rect)
        {
            // Sky
            canvas.RectFilled(rect.x, rect.y, rect.width, rect.height * 0.2, System.Drawing.Color.FromArgb(135, 206, 235));
            
            // Ground
            canvas.RectFilled(rect.x, rect.y + rect.height * 0.2, rect.width, rect.height * 0.8, System.Drawing.Color.FromArgb(124, 185, 82));

            // Draw grass blades
            foreach (var grass in grassBlades)
            {
                double x = rect.x + grass.x * rect.width;
                double y = rect.y + grass.y * rect.height;
                
                canvas.BeginPath();
                canvas.LineTo(x, y);
                canvas.LineTo(x, y - grass.height);
                canvas.SetStrokeColor(System.Drawing.Color.FromArgb(85, 155, 60));
                canvas.SetStrokeWidth(2);
                canvas.Stroke();
            }

            // Draw flowers
            foreach (var flower in flowers)
            {
                double x = rect.x + flower.x * rect.width;
                double y = rect.y + flower.y * rect.height;
                
                // Stem
                canvas.BeginPath();
                canvas.LineTo(x, y);
                canvas.LineTo(x, y - 15);
                canvas.SetStrokeColor(System.Drawing.Color.FromArgb(85, 155, 60));
                canvas.SetStrokeWidth(2);
                canvas.Stroke();
                
                // Petals
                canvas.CircleFilled(x, y - 15, 4, flower.color);
            }
        }

        private void DrawHeart(Canvas canvas, double x, double y, double size)
        {
            System.Drawing.Color heartColor = System.Drawing.Color.FromArgb(255, 105, 180);
            double offset = size * 0.3;
            
            canvas.CircleFilled(x - offset, y, size * 0.5, heartColor);
            canvas.CircleFilled(x + offset, y, size * 0.5, heartColor);
            canvas.CircleFilled(x, y, size * 2, heartColor);
        }

        private void SpawnNewSlime()
        {
            System.Random rand = new System.Random();
            slimes.Add(new Slime(
                new Vector2(rand.Next(50, 500), rand.Next(50, 300)),
                System.Drawing.Color.FromArgb(
                    rand.Next(100, 255),
                    rand.Next(100, 255),
                    rand.Next(100, 255)
                ),
                slimeNames[nameIndex % slimeNames.Length],
                BASE_SLIME_SPEED * (0.7 + rand.NextDouble() * 0.6)
            ));
            nameIndex++;
        }

        public override void Draw()
        {
            using (Gui.Box("Container")
                .Margin(5)
                .Focused
                    .BorderWidth(2)
                    .BorderColor(Themes.primary)
                .End()
                .OnHover((e) =>
                {
                    pointerPosition = e.PointerPosition;
                })
                .Enter())
            {
                Gui.AddActionElement((canvas, rect) =>
                {
                    DrawPrairieBackground(canvas, rect);

                    var deltaTime = Gui.DeltaTime;
                    Vector2 localPointerPosition = new Vector2(
                        pointerPosition.x - rect.x,
                        pointerPosition.y - rect.y
                    );

                    // Handle slime clicks
                    if (Gui.IsPointerPressed(PaperMouseBtn.Left))
                    {
                        for (int i = 0; i < slimes.Count; i++)
                        {
                            var slime = slimes[i];
                            double distance = (localPointerPosition - slime.position).magnitude;
                            
                            if (distance <= SLIME_SIZE)
                            {
                                slime.heartTimer = HEART_DISPLAY_DURATION;
                                slimes[i] = slime;
                            }
                        }
                    }

                    // Update and draw slimes
                    for (int i = 0; i < slimes.Count; i++)
                    {
                        var slime = slimes[i];

                        if (slime.heartTimer > 0)
                        {
                            slime.heartTimer -= deltaTime;
                        }

                        Vector2 direction = localPointerPosition - slime.position;
                        if (direction.magnitude > 1)
                        {
                            direction = direction.normalized;
                            Vector2 moveVector = direction * slime.speed;

                            // SDF-based collision avoidance
                            for (int j = 0; j < slimes.Count; j++)
                            {
                                if (i != j)
                                {
                                    double sdfDistance = CircleSDF(slime.position, slimes[j].position, SLIME_SIZE);
                                    
                                    if (sdfDistance < AVOIDANCE_RADIUS)
                                    {
                                        Vector2 avoidanceDirection = (slime.position - slimes[j].position).normalized;
                                        double avoidanceFactor = Math.Max(0, 1.0 - (sdfDistance / AVOIDANCE_RADIUS));
                                        moveVector += avoidanceDirection * AVOIDANCE_STRENGTH * avoidanceFactor;
                                    }
                                }
                            }

                            slime.position += moveVector * deltaTime;
                        }

                        // Hop animation
                        slime.hopProgress = (slime.hopProgress + deltaTime) % HOP_DURATION;
                        float scale = 1.0f + (float)Math.Sin(slime.hopProgress * (Math.PI * 2 / HOP_DURATION)) * 0.2f;

                        // Draw slime
                        int size = (int)(SLIME_SIZE * scale);
                        canvas.CircleFilled(
                            rect.x + slime.position.x,
                            rect.y + slime.position.y,
                            size,
                            slime.color
                        );

                        // Draw heart if active
                        if (slime.heartTimer > 0)
                        {
                            DrawHeart(
                                canvas,
                                rect.x + slime.position.x,
                                rect.y + slime.position.y - size - 20,
                                10
                            );
                        }

                        // Draw name
                        canvas.DrawText(
                            slime.name,
                            rect.x + slime.position.x,
                            rect.y + slime.position.y - size - 10,
                            System.Drawing.Color.White,
                            nameLayout
                        );

                        slimes[i] = slime;
                    }

                    // Draw spawn button
                    double buttonX = rect.x + SPAWN_BUTTON_X;
                    double buttonY = rect.y + SPAWN_BUTTON_Y;
                });
            }

            // Spawn button click handler
            using (Gui.Box("SpawnButton")
                .PositionType(PositionType.SelfDirected)
                .Left(SPAWN_BUTTON_X)
                .Top(SPAWN_BUTTON_Y)
                .BackgroundColor(System.Drawing.Color.Violet).Rounded(5)
                .Hovered
                    .BackgroundColor(System.Drawing.Color.Pink)
                .End()
                .Text("Spawn Slime", Fonts.arial).Alignment(Prowl.PaperUI.TextAlignment.MiddleCenter)
                .Width(SPAWN_BUTTON_WIDTH)
                .Height(SPAWN_BUTTON_HEIGHT)
                .OnClick((e) => SpawnNewSlime())
                .Enter())
            {

            }
        }

        public override void Focus() { }
        public override void Blur() { }
    }
}