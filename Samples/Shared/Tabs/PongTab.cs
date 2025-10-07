using Prowl.PaperUI;
using Prowl.Quill;
using Prowl.Scribe;

namespace Shared.Tabs
{
    public class PongTab : Tab
    {
        const int BALL_SIZE = 10;
        const int PADDLE_WIDTH = 10;
        const int PADDLE_HEIGHT = 80;
        const double BALL_MAX_VELOCITY = 100.0;
        const double PLAYERS_MAX_VELOCITY = 400.0;
        const double PLAYER_START_VELOCITY = 150.0; // Base velocity for paddle movement

        TextLayoutSettings _scoreLayoutSettings = TextLayoutSettings.Default;

        int player1Score = 0;
        double player1Y = -2;
        bool player1UpPressed = false;
        bool player1DownPressed = false;
        double player1Velocity = 0.0;

        int player2Score = 0;
        double player2Y = -2;
        bool player2UpPressed = false;
        bool player2DownPressed = false;
        double player2Velocity = 0.0;

        bool ballPositionCentered = false;
        bool gameStarted = false;
        double ballX = 0;
        double ballY = 0;
        double ballVelocityX = 0;
        double ballVelocityY = 0;

        Prowl.Vector.Rect _rect;

        public PongTab(Paper gui) : base(gui)
        {
            title = "Pong";
            id = "pong";
            width = 64;
            
            // Larger font for score
            _scoreLayoutSettings.Font = Fonts.arial;
            _scoreLayoutSettings.PixelSize = 24;
            _scoreLayoutSettings.Alignment = Prowl.Scribe.TextAlignment.Center;
        }

        public override void Focus()
        {
            Console.WriteLine("Pong is focused");
        }

        private void ResetBall()
        {
            ballX = _rect.width / 2;
            ballY = _rect.height / 2;
            ballVelocityX = 150 * (Random.Shared.Next(2) * 2 - 1); // Random direction
            ballVelocityY = 150 * (Random.Shared.Next(2) * 2 - 1);
        }

        public override void Blur()
        {
            Console.WriteLine("Pong is blurred");
        }

        double ogBallVelX = 0;
        double ogBallVelY = 0;
        
        public override void Draw()
        {
            using (Gui.Box("Container")
                .Margin(5)
                .Focused
                    .BorderWidth(2)
                    .BorderColor(Themes.primary)
                .End()
                .OnFocusChange((e) =>
                {
                     if (e.IsFocused)
                    {
                        gameStarted = true;
                        ballVelocityX = ogBallVelX; // Random direction
                        ballVelocityY = ogBallVelY;
                    }
                    else
                    {
                        ogBallVelX = ballVelocityX;
                        ogBallVelY = ballVelocityY;
                        ballVelocityX = 0;
                        ballVelocityY = 0;
                    }
                })
                .OnKeyPressed((e) =>
                {
                    if (e.Key == PaperKey.W) player1UpPressed = true;
                    else player1UpPressed = false;
                    if (e.Key == PaperKey.S) player1DownPressed = true;
                    else player1DownPressed = false;

                    if (e.Key == PaperKey.Up) player2UpPressed = true;
                    else player2UpPressed = false;
                    if (e.Key == PaperKey.Down) player2DownPressed = true;
                    else player2DownPressed = false;
                })
                .Enter())
            {
                Gui.AddActionElement((canvas, rect) =>
                {
                    _rect = rect;

                    if (!ballPositionCentered && !gameStarted)
                    {
                        ballX = _rect.width / 2;
                        ballY = _rect.height / 2;
                    }

                    if (gameStarted && !ballPositionCentered)
                    {
                        ResetBall();
                        ballPositionCentered = true;
                    }

                    // Initialize players in the middle if they haven't been positioned yet
                    if (player1Y < -1) player1Y = (rect.height - PADDLE_HEIGHT) / 2;
                    if (player2Y < -1) player2Y = (rect.height - PADDLE_HEIGHT) / 2;

                    var deltaTime = Gui.DeltaTime;

                    // Then modify the velocity calculations in the Draw() method:
                    if (player1UpPressed) player1Velocity = -PLAYER_START_VELOCITY * deltaTime; // Use constant for consistent initial velocity
                    if (player1DownPressed) player1Velocity = PLAYER_START_VELOCITY * deltaTime; // Use constant for consistent initial velocity

                    player1Y += player1Velocity;
                    player1Velocity = Math.Clamp(player1Velocity * 0.98, -PLAYERS_MAX_VELOCITY, PLAYERS_MAX_VELOCITY);

                    if (player2UpPressed) player2Velocity = -PLAYER_START_VELOCITY * deltaTime; // Use constant for consistent initial velocity
                    if (player2DownPressed) player2Velocity = PLAYER_START_VELOCITY * deltaTime; // Use constant for consistent initial velocity

                    player2Y += player2Velocity;
                    player2Velocity = Math.Clamp(player2Velocity * 0.98, -PLAYERS_MAX_VELOCITY, PLAYERS_MAX_VELOCITY);

                    player1Y = Math.Clamp(player1Y, 0, _rect.height - PADDLE_HEIGHT);
                    player2Y = Math.Clamp(player2Y, 0, _rect.height - PADDLE_HEIGHT);

                    // Update ball position
                    ballX += ballVelocityX * deltaTime;
                    ballY += ballVelocityY * deltaTime;

                    // Clamp ball velocity
                    ballVelocityX = Math.Clamp(ballVelocityX, -BALL_MAX_VELOCITY, BALL_MAX_VELOCITY);
                    ballVelocityY = Math.Clamp(ballVelocityY, -BALL_MAX_VELOCITY, BALL_MAX_VELOCITY);

                    // Ball collision with top and bottom
                    if (ballY <= 0 || ballY >= rect.height - BALL_SIZE)
                    {
                        ballVelocityY = -ballVelocityY;
                    }

                    // Ball collision with paddles
                    if (ballX <= PADDLE_WIDTH && ballY >= player1Y && ballY <= player1Y + PADDLE_HEIGHT)
                    {
                        ballVelocityX = Math.Abs(ballVelocityX); // Bounce right
                    }
                    if (ballX >= rect.width - PADDLE_WIDTH - BALL_SIZE && ballY >= player2Y && ballY <= player2Y + PADDLE_HEIGHT)
                    {
                        ballVelocityX = -Math.Abs(ballVelocityX); // Bounce left
                    }

                    // Reset ball and update score if it goes past paddles
                    if (ballX < 0)
                    {
                        player2Score++;
                        ResetBall();
                    }
                    else if (ballX > rect.width)
                    {
                        player1Score++;
                        ResetBall();
                    }

                    player1UpPressed = false;
                    player1DownPressed = false;
                    player2UpPressed = false;
                    player2DownPressed = false;

                    // NOTE (do not remove):
                    // - canvas coordinates are unrestricted and can draw to anywhere on the entire window
                    // - rect is the local coordinates of the parent element, use it to keep the drawing of shapes within the parent UI element

                    // Draw center line
                    // canvas.DashedLine(rect.x + rect.width / 2, rect.y, rect.x + rect.width / 2, rect.y + rect.height, System.Drawing.Color.White, 2, 5);

                    // paddle 1
                    canvas.RectFilled(rect.x, rect.y + player1Y, PADDLE_WIDTH, PADDLE_HEIGHT, System.Drawing.Color.White);

                    // ball (now using ball position)
                    canvas.CircleFilled(rect.x + ballX, rect.y + ballY, BALL_SIZE, System.Drawing.Color.White);

                    // paddle 2
                    canvas.RectFilled(rect.x - PADDLE_WIDTH + rect.width, rect.y + player2Y, PADDLE_WIDTH, PADDLE_HEIGHT, System.Drawing.Color.White);

                    // Draw score at the top
                    canvas.DrawText(player1Score.ToString(), rect.x + (rect.width / 4), rect.y + 30, Themes.baseContent, _scoreLayoutSettings);
                    canvas.DrawText(player2Score.ToString(), rect.x + (rect.width * 3 / 4), rect.y + 30, Themes.baseContent, _scoreLayoutSettings);
                });
            }
        }
    }
}