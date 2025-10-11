using Prowl.PaperUI;
using Prowl.Vector;
using Prowl.Scribe;
using System;
using System.Collections.Generic;

namespace Shared.Tabs
{
    public class AsteroidsTab : Tab
    {
        const int SHIP_SIZE = 15;
        const double SHIP_ROTATION_SPEED = 4.0;
        const double SHIP_THRUST = 300.0;
        const double SHIP_DRAG = 0.98;
        const double SHIP_MAX_SPEED = 400.0;
        const double BULLET_SPEED = 500.0;
        const double BULLET_LIFETIME = 1.5;
        const int MAX_BULLETS = 5;
        const double FIRE_COOLDOWN = 0.2;
        const int STARTING_LIVES = 3;
        const double INVULNERABILITY_TIME = 2.0;
        const double RESPAWN_DELAY = 1.5;
        const int STARTING_ASTEROIDS = 4;

        enum AsteroidSize
        {
            Large,
            Medium,
            Small
        }

        struct Ship
        {
            public Vector2 position;
            public Vector2 velocity;
            public double rotation;
            public bool thrusting;
            public double invulnerabilityTimer;

            public Ship(Vector2 position)
            {
                this.position = position;
                this.velocity = Vector2.zero;
                this.rotation = -Math.PI / 2;
                this.thrusting = false;
                this.invulnerabilityTimer = INVULNERABILITY_TIME;
            }
        }

        struct Bullet
        {
            public Vector2 position;
            public Vector2 velocity;
            public double lifetime;

            public Bullet(Vector2 position, Vector2 velocity)
            {
                this.position = position;
                this.velocity = velocity;
                this.lifetime = BULLET_LIFETIME;
            }
        }

        struct Asteroid
        {
            public Vector2 position;
            public Vector2 velocity;
            public double rotation;
            public double rotationSpeed;
            public AsteroidSize size;
            public Vector2[] shape;

            public Asteroid(Vector2 position, Vector2 velocity, AsteroidSize size, System.Random rand)
            {
                this.position = position;
                this.velocity = velocity;
                this.size = size;
                this.rotation = rand.NextDouble() * Math.PI * 2;
                this.rotationSpeed = (rand.NextDouble() - 0.5) * 2.0;

                // Generate random asteroid shape
                int points = rand.Next(8, 12);
                this.shape = new Vector2[points];
                double baseRadius = GetRadius(size);
                
                for (int i = 0; i < points; i++)
                {
                    double angle = (i / (double)points) * Math.PI * 2;
                    double radius = baseRadius * (0.7 + rand.NextDouble() * 0.3);
                    this.shape[i] = new Vector2(
                        Math.Cos(angle) * radius,
                        Math.Sin(angle) * radius
                    );
                }
            }

            public static double GetRadius(AsteroidSize size)
            {
                return size switch
                {
                    AsteroidSize.Large => 40,
                    AsteroidSize.Medium => 25,
                    AsteroidSize.Small => 15,
                    _ => 40
                };
            }

            public static int GetScore(AsteroidSize size)
            {
                return size switch
                {
                    AsteroidSize.Large => 20,
                    AsteroidSize.Medium => 50,
                    AsteroidSize.Small => 100,
                    _ => 20
                };
            }
        }

        struct Particle
        {
            public Vector2 position;
            public Vector2 velocity;
            public double lifetime;
            public double maxLifetime;

            public Particle(Vector2 position, Vector2 velocity, double lifetime)
            {
                this.position = position;
                this.velocity = velocity;
                this.lifetime = lifetime;
                this.maxLifetime = lifetime;
            }
        }

        Ship ship;
        List<Bullet> bullets = new List<Bullet>();
        List<Asteroid> asteroids = new List<Asteroid>();
        List<Particle> particles = new List<Particle>();
        
        bool leftPressed = false;
        bool rightPressed = false;
        bool thrustPressed = false;
        bool shootPressed = false;
        
        double fireCooldownTimer = 0;
        int score = 0;
        int lives = STARTING_LIVES;
        bool gameOver = false;
        double respawnTimer = 0;
        bool shipActive = true;
        int level = 1;

        Rect _rect;
        System.Random rand = new System.Random();

        TextLayoutSettings scoreLayout = new TextLayoutSettings
        {
            Font = Fonts.arial,
            PixelSize = 20,
            Alignment = Prowl.Scribe.TextAlignment.Left
        };

        TextLayoutSettings gameOverLayout = new TextLayoutSettings
        {
            Font = Fonts.arial,
            PixelSize = 32,
            Alignment = Prowl.Scribe.TextAlignment.Center
        };

        public AsteroidsTab(Paper gui) : base(gui)
        {
            title = "Asteroids";
            id = "asteroids";
            width = 90;
            
            InitializeGame();
        }

        private void InitializeGame()
        {
            ship = new Ship(new Vector2(0, 0));
            bullets.Clear();
            asteroids.Clear();
            particles.Clear();
            score = 0;
            lives = STARTING_LIVES;
            gameOver = false;
            shipActive = true;
            level = 1;
            respawnTimer = 0;
            
            SpawnAsteroids(STARTING_ASTEROIDS);
        }

        private void SpawnAsteroids(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 position;
                do
                {
                    position = new Vector2(
                        rand.NextDouble() * (_rect.width > 0 ? _rect.width : 800),
                        rand.NextDouble() * (_rect.height > 0 ? _rect.height : 600)
                    );
                } while (shipActive && (position - ship.position).magnitude < 150);

                Vector2 velocity = new Vector2(
                    (rand.NextDouble() - 0.5) * 100,
                    (rand.NextDouble() - 0.5) * 100
                );

                asteroids.Add(new Asteroid(position, velocity, AsteroidSize.Large, rand));
            }
        }

        private void SpawnParticles(Vector2 position, int count, double speed)
        {
            for (int i = 0; i < count; i++)
            {
                double angle = rand.NextDouble() * Math.PI * 2;
                Vector2 velocity = new Vector2(
                    Math.Cos(angle) * speed * (0.5 + rand.NextDouble()),
                    Math.Sin(angle) * speed * (0.5 + rand.NextDouble())
                );
                particles.Add(new Particle(position, velocity, 0.5 + rand.NextDouble() * 0.5));
            }
        }

        private void RespawnShip()
        {
            ship = new Ship(new Vector2(_rect.width / 2, _rect.height / 2));
            shipActive = true;
        }

        private void DestroyShip()
        {
            SpawnParticles(ship.position, 20, 150);
            shipActive = false;
            lives--;
            
            if (lives <= 0)
            {
                gameOver = true;
            }
            else
            {
                respawnTimer = RESPAWN_DELAY;
            }
        }

        private Vector2 WrapPosition(Vector2 position)
        {
            double x = position.x;
            double y = position.y;

            if (x < 0) x = _rect.width;
            if (x > _rect.width) x = 0;
            if (y < 0) y = _rect.height;
            if (y > _rect.height) y = 0;

            return new Vector2(x, y);
        }

        private void SplitAsteroid(Asteroid asteroid)
        {
            score += Asteroid.GetScore(asteroid.size);
            SpawnParticles(asteroid.position, 10, 100);

            if (asteroid.size != AsteroidSize.Small)
            {
                AsteroidSize newSize = asteroid.size == AsteroidSize.Large ? 
                    AsteroidSize.Medium : AsteroidSize.Small;

                for (int i = 0; i < 2; i++)
                {
                    double angle = rand.NextDouble() * Math.PI * 2;
                    Vector2 velocity = new Vector2(
                        Math.Cos(angle) * 120,
                        Math.Sin(angle) * 120
                    );
                    asteroids.Add(new Asteroid(asteroid.position, velocity, newSize, rand));
                }
            }
        }

        public override void Focus()
        {
            Console.WriteLine("Asteroids is focused");
        }

        public override void Blur()
        {
            Console.WriteLine("Asteroids is blurred");
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
                    if (e.Key == PaperKey.A || e.Key == PaperKey.Left) leftPressed = true;
                    if (e.Key == PaperKey.D || e.Key == PaperKey.Right) rightPressed = true;
                    if (e.Key == PaperKey.W || e.Key == PaperKey.Up) thrustPressed = true;
                    if (e.Key == PaperKey.Space) shootPressed = true;
                    if (e.Key == PaperKey.R && gameOver) InitializeGame();
                })
                .Enter())
            {
                Gui.AddActionElement((canvas, rect) =>
                {
                    _rect = rect;
                    var deltaTime = Gui.DeltaTime;

                    // Initialize ship position if needed
                    if (ship.position.x == 0 && ship.position.y == 0)
                    {
                        ship.position = new Vector2(rect.width / 2, rect.height / 2);
                    }

                    if (!gameOver)
                    {
                        // Handle respawn
                        if (!shipActive)
                        {
                            respawnTimer -= deltaTime;
                            if (respawnTimer <= 0)
                            {
                                RespawnShip();
                            }
                        }

                        // Update ship
                        if (shipActive)
                        {
                            if (leftPressed) ship.rotation -= SHIP_ROTATION_SPEED * deltaTime;
                            if (rightPressed) ship.rotation += SHIP_ROTATION_SPEED * deltaTime;

                            ship.thrusting = thrustPressed;

                            if (thrustPressed)
                            {
                                Vector2 thrust = new Vector2(
                                    Math.Cos(ship.rotation),
                                    Math.Sin(ship.rotation)
                                ) * SHIP_THRUST * deltaTime;
                                ship.velocity += thrust;

                                // Spawn thrust particles
                                if (rand.NextDouble() < 0.5)
                                {
                                    Vector2 exhaustPos = ship.position - new Vector2(
                                        Math.Cos(ship.rotation) * SHIP_SIZE,
                                        Math.Sin(ship.rotation) * SHIP_SIZE
                                    );
                                    SpawnParticles(exhaustPos, 1, 50);
                                }
                            }

                            ship.velocity *= SHIP_DRAG;
                            double speed = ship.velocity.magnitude;
                            if (speed > SHIP_MAX_SPEED)
                            {
                                ship.velocity = ship.velocity.normalized * SHIP_MAX_SPEED;
                            }

                            ship.position += ship.velocity * deltaTime;
                            ship.position = WrapPosition(ship.position);

                            if (ship.invulnerabilityTimer > 0)
                            {
                                ship.invulnerabilityTimer -= deltaTime;
                            }

                            // Shooting
                            if (shootPressed && fireCooldownTimer <= 0 && bullets.Count < MAX_BULLETS)
                            {
                                Vector2 bulletVelocity = new Vector2(
                                    Math.Cos(ship.rotation),
                                    Math.Sin(ship.rotation)
                                ) * BULLET_SPEED;
                                
                                bullets.Add(new Bullet(ship.position, bulletVelocity));
                                fireCooldownTimer = FIRE_COOLDOWN;
                            }
                        }

                        // Update fire cooldown
                        if (fireCooldownTimer > 0)
                        {
                            fireCooldownTimer -= deltaTime;
                        }

                        // Update bullets
                        for (int i = bullets.Count - 1; i >= 0; i--)
                        {
                            var bullet = bullets[i];
                            bullet.position += bullet.velocity * deltaTime;
                            bullet.position = WrapPosition(bullet.position);
                            bullet.lifetime -= deltaTime;

                            if (bullet.lifetime <= 0)
                            {
                                bullets.RemoveAt(i);
                            }
                            else
                            {
                                bullets[i] = bullet;
                            }
                        }

                        // Update asteroids
                        for (int i = 0; i < asteroids.Count; i++)
                        {
                            var asteroid = asteroids[i];
                            asteroid.position += asteroid.velocity * deltaTime;
                            asteroid.position = WrapPosition(asteroid.position);
                            asteroid.rotation += asteroid.rotationSpeed * deltaTime;
                            asteroids[i] = asteroid;
                        }

                        // Check bullet-asteroid collisions
                        for (int i = bullets.Count - 1; i >= 0; i--)
                        {
                            for (int j = asteroids.Count - 1; j >= 0; j--)
                            {
                                double distance = (bullets[i].position - asteroids[j].position).magnitude;
                                if (distance < Asteroid.GetRadius(asteroids[j].size))
                                {
                                    SplitAsteroid(asteroids[j]);
                                    asteroids.RemoveAt(j);
                                    bullets.RemoveAt(i);
                                    break;
                                }
                            }
                        }

                        // Check ship-asteroid collisions
                        if (shipActive && ship.invulnerabilityTimer <= 0)
                        {
                            for (int i = 0; i < asteroids.Count; i++)
                            {
                                double distance = (ship.position - asteroids[i].position).magnitude;
                                if (distance < SHIP_SIZE + Asteroid.GetRadius(asteroids[i].size))
                                {
                                    DestroyShip();
                                    break;
                                }
                            }
                        }

                        // Update particles
                        for (int i = particles.Count - 1; i >= 0; i--)
                        {
                            var particle = particles[i];
                            particle.position += particle.velocity * deltaTime;
                            particle.lifetime -= deltaTime;

                            if (particle.lifetime <= 0)
                            {
                                particles.RemoveAt(i);
                            }
                            else
                            {
                                particles[i] = particle;
                            }
                        }

                        // Check for level complete
                        if (asteroids.Count == 0)
                        {
                            level++;
                            SpawnAsteroids(STARTING_ASTEROIDS + level - 1);
                        }
                    }

                    // Draw background
                    canvas.RectFilled(rect.x, rect.y, rect.width, rect.height, System.Drawing.Color.Black);

                    // Draw particles
                    foreach (var particle in particles)
                    {
                        double alpha = particle.lifetime / particle.maxLifetime;
                        canvas.CircleFilled(
                            rect.x + particle.position.x,
                            rect.y + particle.position.y,
                            2,
                            System.Drawing.Color.FromArgb((int)(255 * alpha), 255, 255, 255)
                        );
                    }

                    // Draw asteroids
                    foreach (var asteroid in asteroids)
                    {
                        canvas.BeginPath();
                        for (int i = 0; i < asteroid.shape.Length; i++)
                        {
                            double cos = Math.Cos(asteroid.rotation);
                            double sin = Math.Sin(asteroid.rotation);
                            double x = asteroid.shape[i].x * cos - asteroid.shape[i].y * sin;
                            double y = asteroid.shape[i].x * sin + asteroid.shape[i].y * cos;

                            if (i == 0)
                                canvas.MoveTo(rect.x + asteroid.position.x + x, rect.y + asteroid.position.y + y);
                            else
                                canvas.LineTo(rect.x + asteroid.position.x + x, rect.y + asteroid.position.y + y);
                        }
                        canvas.ClosePath();
                        canvas.SetStrokeColor(System.Drawing.Color.White);
                        canvas.SetStrokeWidth(2);
                        canvas.Stroke();
                    }

                    // Draw ship
                    if (shipActive && (ship.invulnerabilityTimer <= 0 || ((int)(ship.invulnerabilityTimer * 10) % 2 == 0)))
                    {
                        Vector2 front = new Vector2(
                            Math.Cos(ship.rotation) * SHIP_SIZE,
                            Math.Sin(ship.rotation) * SHIP_SIZE
                        );
                        Vector2 back1 = new Vector2(
                            Math.Cos(ship.rotation + 2.5) * SHIP_SIZE * 0.7,
                            Math.Sin(ship.rotation + 2.5) * SHIP_SIZE * 0.7
                        );
                        Vector2 back2 = new Vector2(
                            Math.Cos(ship.rotation - 2.5) * SHIP_SIZE * 0.7,
                            Math.Sin(ship.rotation - 2.5) * SHIP_SIZE * 0.7
                        );

                        canvas.BeginPath();
                        canvas.MoveTo(rect.x + ship.position.x + front.x, rect.y + ship.position.y + front.y);
                        canvas.LineTo(rect.x + ship.position.x + back1.x, rect.y + ship.position.y + back1.y);
                        canvas.LineTo(rect.x + ship.position.x + back2.x, rect.y + ship.position.y + back2.y);
                        canvas.ClosePath();
                        canvas.SetStrokeColor(System.Drawing.Color.White);
                        canvas.SetStrokeWidth(2);
                        canvas.Stroke();

                        // Draw thrust
                        if (ship.thrusting)
                        {
                            Vector2 thrustPoint = new Vector2(
                                Math.Cos(ship.rotation + Math.PI) * SHIP_SIZE * 0.8,
                                Math.Sin(ship.rotation + Math.PI) * SHIP_SIZE * 0.8
                            );
                            canvas.BeginPath();
                            canvas.MoveTo(rect.x + ship.position.x + back1.x, rect.y + ship.position.y + back1.y);
                            canvas.LineTo(rect.x + ship.position.x + thrustPoint.x, rect.y + ship.position.y + thrustPoint.y);
                            canvas.LineTo(rect.x + ship.position.x + back2.x, rect.y + ship.position.y + back2.y);
                            canvas.SetStrokeColor(System.Drawing.Color.Orange);
                            canvas.SetStrokeWidth(2);
                            canvas.Stroke();
                        }
                    }

                    // Draw bullets
                    foreach (var bullet in bullets)
                    {
                        canvas.CircleFilled(rect.x + bullet.position.x, rect.y + bullet.position.y, 2, System.Drawing.Color.White);
                    }

                    // Draw UI
                    canvas.DrawText($"Score: {score}", rect.x + 10, rect.y + 25, System.Drawing.Color.White, scoreLayout);
                    canvas.DrawText($"Lives: {lives}", rect.x + 10, rect.y + 50, System.Drawing.Color.White, scoreLayout);
                    canvas.DrawText($"Level: {level}", rect.x + 10, rect.y + 75, System.Drawing.Color.White, scoreLayout);

                    // Draw game over
                    if (gameOver)
                    {
                        canvas.DrawText("GAME OVER", rect.x + rect.width / 2, rect.y + rect.height / 2 - 20, System.Drawing.Color.White, gameOverLayout);
                        canvas.DrawText($"Final Score: {score}", rect.x + rect.width / 2, rect.y + rect.height / 2 + 20, System.Drawing.Color.White, scoreLayout);
                        canvas.DrawText("Press R to Restart", rect.x + rect.width / 2, rect.y + rect.height / 2 + 50, System.Drawing.Color.White, scoreLayout);
                    }

                    leftPressed = false;
                    rightPressed = false;
                    thrustPressed = false;
                    shootPressed = false;
                });
            }
        }
    }
}