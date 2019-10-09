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
    public enum SceneType
    {
        Menu,
        Platformer,      
        Flocking,
    }

    static class SceneManager
    {
        public static Scene CurrentScene { get; private set; }
        public static Scene NextScene { get; private set; }

        private static List<Scene> scenes;
        private static Transition enterTransition;
        private static Transition exitTransition;
        private static bool transitionInProgress;

        public static void Initialize()
        {
            PreLoadScenes();
            SetStartingScene(SceneType.Menu);
        }

        private static void PreLoadScenes()
        {
            scenes = new List<Scene>()
            {       
                new Menu(),
                new Platformer(),
                new Flocking(),
            };
        }

        private static void SetStartingScene(SceneType scene)
        {
            QueueScene(scene);
        }

        private static Scene ParseSceneType(SceneType scene)
        {
            foreach (Scene s in scenes)
            {
                if (s.SceneType == scene)
                    return s;
            }
            return null;
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

        public static void QueueScene(SceneType scene)
        {
            NextScene = ParseSceneType(scene);
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

            if (exitTransition != null && exitTransition.InProgress)
            {
                exitTransition.Update();
            }
            else if (((exitTransition != null && exitTransition.Done) || (exitTransition == null)) && !enterTransition.Started)
            {
                UnloadCurrentScene();
                LoadNextScene();
                enterTransition.Start();
            }
            else if (enterTransition.InProgress)
            {
                enterTransition.Update();
            }
            else if (enterTransition.Done)
            {
                transitionInProgress = false;
            }
        }

        private static void UpdateCurrentScene(GameTime gameTime)
        {
            if (transitionInProgress)
                return;

            CurrentScene.Update(gameTime);
        }

        public static void Update(GameTime gameTime)
        {
            UpdateTransitions();
            UpdateCurrentScene(gameTime);

            if (Input.Keyboard.Pressed(Keys.R))
            {
                CurrentScene.LoadScene();
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            CurrentScene.Draw(spriteBatch);

            Sketch.Begin(spriteBatch);
            {
                if (exitTransition != null)
                    exitTransition.Draw(spriteBatch);
                if (enterTransition != null)
                    enterTransition.Draw(spriteBatch);
            }
            Sketch.End(spriteBatch);
        }
    }
}
