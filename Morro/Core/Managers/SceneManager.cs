﻿using Microsoft.Xna.Framework;
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
    /// <summary>
    /// Handles the entire life cycle of any registered <see cref="Scene"/>'s.
    /// </summary>
    static class SceneManager
    {
        public static Scene CurrentScene { get; private set; }
        public static Scene NextScene { get; private set; }

        private static readonly ResourceHandler<Scene> scenes;
        private static Transition enterTransition;
        private static Transition exitTransition;
        private static bool transitionInProgress;
        private static bool exitCompleted;

        static SceneManager()
        {
            scenes = new ResourceHandler<Scene>();
        }

        #region Handle Scenes
        /// <summary>
        /// Register a <see cref="Scene"/> to be managed by Morro.
        /// </summary>
        /// <param name="scene">The scene you want to be registered.</param>
        public static void RegisterScene(Scene scene)
        {
            scenes.Register(scene.Name, scene);
        }

        /// <summary>
        /// Get a <see cref="Scene"/> that was previously registered.
        /// </summary>
        /// <param name="name">The name given to the scene that was previously registered.</param>
        /// <returns>The registered scene with the given name.</returns>
        public static Scene GetScene(string name)
        {
            return scenes.Get(name);
        }

        /// <summary>
        /// Remove a registered <see cref="Scene"/>.
        /// </summary>
        /// <param name="name">The name given to the scene that was previously registered.</param>
        public static void RemoveScene(string name)
        {
            scenes.Remove(name);
        }
        #endregion

        /// <summary>
        /// Queue a <see cref="Scene"/>, and start the <see cref="Transition"/> between the current scene and the given scene.
        /// </summary>
        /// <param name="name">The name given to the scene that was previously registered.</param>
        public static void QueueScene(string name)
        {
            if (transitionInProgress)
                return;

            NextScene = GetScene(name);
            SetupTransitions();
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
                exitTransition.Begin();
            }

            transitionInProgress = true;
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
                    enterTransition.Begin();

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
                        enterTransition.Begin();

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

        private static void DrawTransitions()
        {
            if (!transitionInProgress)
                return;

            Sketch.Begin();
            {
                if (!exitCompleted)
                {
                    exitTransition?.Draw();
                }
                else
                {
                    enterTransition?.Draw();
                }
            }
            Sketch.End();
        }

        internal static void Update()
        {
            UpdateTransitions();
            UpdateCurrentScene();

            if (DebugManager.Debugging && Input.Keyboard.Pressed(Keys.R))
            {
                CurrentScene?.LoadScene();
            }
        }

        internal static void Draw()
        {
            CurrentScene?.Draw();
            DrawTransitions();
        }
    }
}
