using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Morro.Core;
using Morro.ECS.Scenes;
using Morro.Graphics;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS.Entities
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
            animatedSprite = new AnimatedSprite(X - 3, Y - 2, SpriteType.Block, AnimationType.Loop, 6, 6, 100, false);

            jumpHeight = 64 + 4;
            jumpDuration = 0.5f;

            gravity = 2 * jumpHeight / (jumpDuration * jumpDuration);
            
            initialJumpVelocity = gravity * jumpDuration;
            Acceleration = new Vector2(0, gravity);

            lateralAcceleration = 150;
            friction = 100;
            drag = 50;

            Acceleration = new Vector2(0, gravity);
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
                

            foreach (Polygon polygon in ((Platformer)SceneManager.CurrentScene).BoundingBoxes)
            {
                if (polygon is AABB)
                {
                    ResolveCollisionAgainst((AABB)polygon);
                    foreach (CollisionType collisionType in collisionTypes)
                    {
                        switch (collisionType)
                        {
                            case CollisionType.QuadRight:
                                Velocity = new Vector2(-Velocity.X * 0.75f, Velocity.Y);
                                break;

                            case CollisionType.QuadLeft:
                                Velocity = new Vector2(-Velocity.X * 0.75f, Velocity.Y);
                                break;

                            case CollisionType.QuadTop:
                                Velocity = new Vector2(Velocity.X, 0);
                                grounded = true;
                                jumping = false;
                                break;

                            case CollisionType.QuadBottom:
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
                        }
                    }
                }

                if (polygon is RightTriangle)
                {
                    ResolveCollisionAgainst((RightTriangle)polygon);

                    foreach (CollisionType collisionType in collisionTypes)
                    {
                        switch (collisionType)
                        {
                            case CollisionType.RightTriangleSlopeTop:
                                Velocity = new Vector2(Velocity.X, 0);
                                grounded = true;
                                jumping = false;
                                break;
                        }
                    }
                }
            }               
        }

        protected override void UpdateInput()
        {
            if (Input.Keyboard.Pressing(Keys.A))
            {
                Acceleration = new Vector2(-lateralAcceleration, Acceleration.Y);
                if (Velocity.X < -MoveSpeed)
                    Velocity = new Vector2(-MoveSpeed, Velocity.Y);

                animatedSprite.PlayAnimation();
                animatedSprite.SpriteEffect = SpriteEffects.FlipHorizontally;
            }
            if (Input.Keyboard.Pressing(Keys.D))
            {
                Acceleration = new Vector2(lateralAcceleration, Acceleration.Y);
                if (Velocity.X > MoveSpeed)
                    Velocity = new Vector2(MoveSpeed, Velocity.Y);

                animatedSprite.PlayAnimation();
                animatedSprite.SpriteEffect = SpriteEffects.None;
            }
            if (!Input.Keyboard.Pressing(Keys.A) && !Input.Keyboard.Pressing(Keys.D))
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

            if (grounded && !jumping && Input.Keyboard.Pressing(Keys.Space))
            {
                Velocity = new Vector2(Velocity.X, -initialJumpVelocity);
                jumping = true;
            }
            if (jumping && Velocity.Y < -initialJumpVelocity / 2 && !Input.Keyboard.Pressing(Keys.Space))
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
