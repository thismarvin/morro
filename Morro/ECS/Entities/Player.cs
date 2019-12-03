using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.Graphics;
using Morro.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    class Player : Actor
    {
        private bool grounded;
        private bool jumping;

        private readonly float jumpHeight;
        private readonly float jumpDuration;
        private readonly float initialJumpVelocity;
        private readonly float gravity;

        private readonly float lateralAcceleration;
        private readonly float friction;
        private readonly float drag;

        private readonly AnimatedSprite animatedSprite;

        public Player(float x, float y, PlayerIndex playerIndex) : base(x, y, 10, 28, 75, playerIndex)
        {
            animatedSprite = new AnimatedSprite(X - 3, Y - 2, "Player", AnimationType.Loop, 6, 6, 100, false);

            jumpHeight = 64 + 4;
            jumpDuration = 0.5f;

            gravity = 2 * jumpHeight / (jumpDuration * jumpDuration);

            initialJumpVelocity = gravity * jumpDuration;
            Acceleration = new Vector2(0, gravity);

            lateralAcceleration = 150;
            friction = 100;
            drag = 50;

            inputHandler.LoadProfile("Player");
        }

        public override void SetLocation(float x, float y)
        {
            base.SetLocation(x, y);
            animatedSprite.SetLocation(X - 3, Y - 2);
        }

        protected override void Collision()
        {
            grounded = false;

            if (Bounds.Left < 0)
            {
                SetLocation(0, Y);
                Velocity = new Vector2(-Velocity.X * 0.75f, Velocity.Y);
            }

            if (Bounds.Right > SceneManager.CurrentScene.SceneBounds.Width)
            {
                SetLocation(SceneManager.CurrentScene.SceneBounds.Width - Width, Y);
                Velocity = new Vector2(-Velocity.X * 0.75f, Velocity.Y);
            }

            List<CollisionType> collisionTypes;
            foreach (Polygon polygon in ((Platformer)SceneManager.CurrentScene).BoundingBoxes)
            {
                collisionTypes = ResolveCollisionAgainst(polygon);
                foreach (CollisionType collisionType in collisionTypes)
                {
                    switch (collisionType)
                    {
                        case CollisionType.Right:
                            Velocity = new Vector2(-Velocity.X * 0.75f, Velocity.Y);
                            break;

                        case CollisionType.Left:
                            Velocity = new Vector2(-Velocity.X * 0.75f, Velocity.Y);
                            break;

                        case CollisionType.Top:
                            Velocity = new Vector2(Velocity.X, 0);
                            grounded = true;
                            jumping = false;
                            break;

                        case CollisionType.Bottom:
                            if (Bounds.Right - polygon.Bounds.Left <= 4)
                            {
                                SetLocation(polygon.Bounds.Left - Width, Y);
                            }
                            else if (polygon.Bounds.Right - Bounds.Left <= 4)
                            {
                                SetLocation(polygon.Bounds.Right, Y);
                            }
                            else
                            {
                                Velocity = new Vector2(Velocity.X, 0);
                            }
                            break;

                        case CollisionType.Slope:
                            Velocity = new Vector2(Velocity.X, 0);
                            grounded = true;
                            jumping = false;
                            break;
                    }
                }
            }
        }

        protected override void UpdateInput()
        {
            base.UpdateInput();

            if (inputHandler.Pressing("Left"))
            {
                Acceleration = new Vector2(-lateralAcceleration, Acceleration.Y);
                if (Velocity.X < -MoveSpeed)
                    Velocity = new Vector2(-MoveSpeed, Velocity.Y);

                animatedSprite.PlayAnimation();
                animatedSprite.SpriteEffect = SpriteEffects.FlipHorizontally;
            }
            if (inputHandler.Pressing("Right"))
            {
                Acceleration = new Vector2(lateralAcceleration, Acceleration.Y);
                if (Velocity.X > MoveSpeed)
                    Velocity = new Vector2(MoveSpeed, Velocity.Y);

                animatedSprite.PlayAnimation();
                animatedSprite.SpriteEffect = SpriteEffects.None;
            }
            if (!(inputHandler.Pressing("Left")) && !(inputHandler.Pressing("Right")))
            {
                if (Velocity.X != 0)
                {
                    float dir = Velocity.X > 0 ? -1 : 1;
                    if (grounded)
                        Acceleration = new Vector2(friction * dir, Acceleration.Y);
                    else
                        Acceleration = new Vector2(drag * dir, Acceleration.Y);

                    if (-1 < Velocity.X && Velocity.X < 1)
                    {
                        Velocity = new Vector2(0, Velocity.Y);
                        Acceleration = new Vector2(0, Acceleration.Y);
                    }
                }

                animatedSprite.PauseAnimation();
                animatedSprite.ResetAnimation();
            }

            if (grounded && !jumping && inputHandler.Pressing("Jump"))
            {
                Velocity = new Vector2(Velocity.X, -initialJumpVelocity);
                jumping = true;
            }
            if (jumping && Velocity.Y < -initialJumpVelocity / 2 && !inputHandler.Pressing("Jump"))
            {
                Velocity = new Vector2(Velocity.X, -initialJumpVelocity / 2);
                jumping = false;
            }
        }

        public override void Update(GameTime gameTime)
        {
            UpdateInput();
            ApplyForce(Integrator.VelocityVerlet);
            Collision();

            animatedSprite.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            animatedSprite.Draw(spriteBatch, CameraType.Dynamic);
        }
    }
}
