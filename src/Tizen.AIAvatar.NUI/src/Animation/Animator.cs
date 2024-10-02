/*
 * Copyright(c) 2024 Samsung Electronics Co., Ltd.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

using System;
using System.Collections.Generic;
using Tizen.NUI;

namespace Tizen.AIAvatar
{
    internal abstract class Animator : IDisposable
    {
        protected bool isEnabled = false;

        protected readonly object eventHandlerLock = new object();
        protected event EventHandler<AnimatorChangedEventArgs> animatorStateChanged;
        protected AnimatorState currentAnimatorState = AnimatorState.Unavailable;

        protected Dictionary<int, Animation> animations = new Dictionary<int, Animation>();

        public int Add(Animation animation)
        {
            int index = GetNextIndex();
            animations.Add(index, animation);
            return index;
        }

        public abstract void Play(int index);

        public abstract void Stop(int index);

        public abstract void Pause(int index);

        public void Remove(int index)
        {
            if (!animations.ContainsKey(index))
            {
                throw new ArgumentException($"Animation with index {index} does not exist.");
            }
            animations.Remove(index);
        }

        public virtual void Dispose()
        {
            foreach (var animation in animations.Values)
            {
                animation.Dispose();
            }
            animations.Clear();
        }

        public void SetDebugState(bool enable)
        {
            if (enable && !isEnabled)
            {
                AnimatorStateChanged += OnAnimatorStateChanged;
                isEnabled = true;
            }
            else if (!enable && isEnabled)
            {
                AnimatorStateChanged -= OnAnimatorStateChanged;
                isEnabled = false;
            }
        }

        public AnimatorState CurrentAnimatorState
        {
            get => currentAnimatorState;
            set
            {
                if (currentAnimatorState == value) return;
                var preState = currentAnimatorState;
                currentAnimatorState = value;
                animatorStateChanged?.Invoke(this, new AnimatorChangedEventArgs(preState, currentAnimatorState));
            }
        }

        public event EventHandler<AnimatorChangedEventArgs> AnimatorStateChanged
        {
            add
            {
                lock (eventHandlerLock)
                {
                    animatorStateChanged += value;
                }
            }
            remove
            {
                lock (eventHandlerLock)
                {
                    if (animatorStateChanged == null)
                    {
                        Log.Error("Tizen.AIAvatar", "Remove StateChanged Failed : motionChanged is null");
                        return;
                    }
                    animatorStateChanged -= value;
                }
            }
        }

        protected void OnAnimatorStateChanged(object sender, AnimatorChangedEventArgs e)
        {
            Log.Info("Tizen.AIAvatar", $"Animator state changed from {e.Previous} to {e.Current}");
        }

        protected int GetNextIndex()
        {
            int index = 0;
            while (animations.ContainsKey(index))
            {
                index++;
            }
            return index;
        }
    }
}
