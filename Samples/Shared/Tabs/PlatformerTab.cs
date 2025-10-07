using Prowl.PaperUI;
using Prowl.Vector;

namespace Shared.Tabs
{
    public class PlatformerTab : Tab
    {
        const int PLAYER_SIZE = 20;
        const double PLAYER_ACCELERATION = 1200.0;
        const double PLAYER_MAX_SPEED = 200.0;
        const double PLAYER_FRICTION = 0.93;
        const double JUMP_FORCE = -400.0;
        const double GRAVITY = 800.0;

        double playerX = 100;
        double playerY = 0;
        double velocityX = 0;
        double velocityY = 0;
        bool isGrounded = false;
        bool leftPressed = false;
        bool rightPressed = false;

        Rect _rect;

        public PlatformerTab(Paper gui) : base(gui)
        {
            title = "Platformer";
            id = "platformer";
            width = 90;
        }

        public override void Focus()
        {
            Console.WriteLine("Platformer is focused");
        }

        public override void Blur()
        {
            Console.WriteLine("Platformer is blurred");
        }

        struct Platform
        {
            public double x;
            public double y;
            public double width;
            public double height;

            public Platform(double x, double y, double width, double height)
            {
                this.x = x;
                this.y = y;
                this.width = width;
                this.height = height;
            }
        }

        Platform[] platforms = new Platform[] {
            new Platform(100, 300, 100, 20),
            new Platform(250, 250, 100, 20),
            new Platform(400, 200, 100, 20),
            new Platform(200, 150, 100, 20),
        };

        private bool CheckCollision(double x, double y, Platform platform)
        {
            return x < platform.x + platform.width &&
                   x + PLAYER_SIZE > platform.x &&
                   y < platform.y + platform.height &&
                   y + PLAYER_SIZE > platform.y;
        }

        public override void Draw()
        {
            using (Gui.Box("Container")
            .Margin(5)
            .Focused
                .BorderWidth(2)
                .BorderColor(Themes.primary)
            .End()
            .OnKeyPressed((e) =>
            {
                if (e.Key == PaperKey.A) leftPressed = true;
                if (e.Key == PaperKey.D) rightPressed = true;
                if (e.Key == PaperKey.Space && isGrounded) velocityY = JUMP_FORCE;
            })
            .Enter())
            {
                Gui.AddActionElement((canvas, rect) =>
                {
                    _rect = rect;
                    var deltaTime = Gui.DeltaTime;

                    // Horizontal movement with acceleration
                    if (leftPressed) velocityX -= PLAYER_ACCELERATION * deltaTime;
                    if (rightPressed) velocityX += PLAYER_ACCELERATION * deltaTime;

                    // Apply friction when no keys are pressed
                    velocityX *= PLAYER_FRICTION;

                    // Clamp horizontal speed
                    velocityX = Math.Clamp(velocityX, -PLAYER_MAX_SPEED, PLAYER_MAX_SPEED);

                    // Apply gravity
                    velocityY += GRAVITY * deltaTime;

                    // Update position
                    double newPlayerX = playerX + velocityX * deltaTime;
                    double newPlayerY = playerY + velocityY * deltaTime;

                    // Platform collision detection
                    isGrounded = false;
                    foreach (var platform in platforms)
                    {
                        if (CheckCollision(newPlayerX, newPlayerY, platform))
                        {
                            // Vertical collision
                            if (velocityY > 0 && playerY + PLAYER_SIZE <= platform.y + 5)
                            {
                                newPlayerY = platform.y - PLAYER_SIZE;
                                velocityY = 0;
                                isGrounded = true;
                            }
                            // Head collision
                            else if (velocityY < 0 && playerY >= platform.y + platform.height - 5)
                            {
                                newPlayerY = platform.y + platform.height;
                                velocityY = 0;
                            }
                            // Horizontal collision
                            else if (velocityX != 0)
                            {
                                newPlayerX = velocityX > 0 ? 
                                    platform.x - PLAYER_SIZE : 
                                    platform.x + platform.width;
                                velocityX = 0;
                            }
                        }
                    }

                    // Ground collision
                    if (newPlayerY + PLAYER_SIZE >= rect.height)
                    {
                        newPlayerY = rect.height - PLAYER_SIZE;
                        velocityY = 0;
                        isGrounded = true;
                    }

                    // Wall collision
                    newPlayerX = Math.Clamp(newPlayerX, 0, rect.width - PLAYER_SIZE);

                    // Update position
                    playerX = newPlayerX;
                    playerY = newPlayerY;

                    // Draw platforms
                    foreach (var platform in platforms)
                    {
                        canvas.RectFilled(
                            rect.x + platform.x, 
                            rect.y + platform.y, 
                            platform.width, 
                            platform.height, 
                            System.Drawing.Color.Gray
                        );
                    }

                    // Draw player
                    canvas.RectFilled(rect.x + playerX, rect.y + playerY, PLAYER_SIZE, PLAYER_SIZE, System.Drawing.Color.White);

                    // Draw ground
                    canvas.RectFilled(rect.x, rect.y + rect.height - 2, rect.width, 2, System.Drawing.Color.White);

                    leftPressed = false;
                    rightPressed = false;
                });
            }
        }
    }
}