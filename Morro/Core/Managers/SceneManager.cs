using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Morro.ECS;
using Morro.Graphics;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    static class SceneManager
    {
        public static Scene CurrentScene { get; private set; }
        public static Scene NextScene { get; private set; }

        private static Transition enterTransition;
        private static Transition exitTransition;
        private static bool transitionInProgress;
        private static bool exitCompleted;

        private static Dictionary<string, Scene> scenes;

        public static void Initialize()
        {
            scenes = new Dictionary<string, Scene>();
        }

        public static void RegisterScene(Scene scene)
        {
            scenes.Add(scene.Name, scene);
        }

        private static void SetupTransitions()
        {
            if (CurrentScene == null)
            {
                exitTransition = null;
                enterTransition = NextScene.EnterTransition;
            }
            else
            {
                exitTransition = CurrentScene.ExitTransition;
                enterTransition = NextScene.EnterTransition;
                exitTransition.Start();
            }

            transitionInProgress = true;
        }

        public static void QueueScene(string scene)
        {
            if (!scenes.ContainsKey(scene.ToLowerInvariant()))
                throw new Exception("A scene with that name has not been registered.");

            if (transitionInProgress)
                return;

            NextScene = scenes[scene.ToLowerInvariant()];
            SetupTransitions();
        }

        private static void UnloadCurrentScene()
        {
            if (CurrentScene == null)
                return;

            CurrentScene.UnloadScene();
        }

        private static void LoadNextScene()
        {
            CurrentScene = NextScene;
            CurrentScene.LoadScene();
            NextScene = null;
        }

        private static void UpdateTransitions()
        {
            if (!transitionInProgress)
                return;

            if (!exitCompleted)
            {
                if (exitTransition == null)
                {
                    LoadNextScene();
                    enterTransition.Start();

                    exitCompleted = true;
                }
                else
                {
                    exitTransition.Update();

                    if (exitTransition.Done)
                    {
                        UnloadCurrentScene();
                        exitTransition.Reset();
                        exitTransition = null;

                        LoadNextScene();
                        enterTransition.Start();

                        exitCompleted = true;
                    }
                }
            }
            else
            {
                if (enterTransition != null)
                {
                    enterTransition.Update();

                    if (enterTransition.Done)
                    {
                        enterTransition.Reset();
                        enterTransition = null;

                        transitionInProgress = false;
                        exitCompleted = false;
                    }
                }
            }
        }

        private static void UpdateCurrentScene()
        {
            CurrentScene?.Update();
        }

        private static void DrawTransitions(SpriteBatch spriteBatch)
        {
            if (!transitionInProgress)
                return;

            Sketch.Begin(spriteBatch);
            {
                if (!exitCompleted)
                {
                    exitTransition?.Draw(spriteBatch);
                }
                else
                {
                    enterTransition?.Draw(spriteBatch);
                }
            }
            Sketch.End(spriteBatch);
        }

        public static void Update()
        {
            UpdateTransitions();
            UpdateCurrentScene();

            if (Input.Keyboard.Pressed(Keys.R))
            {
                CurrentScene?.LoadScene();
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            CurrentScene?.Draw(spriteBatch);
            DrawTransitions(spriteBatch);
        }
    }
}
