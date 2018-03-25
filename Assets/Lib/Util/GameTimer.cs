using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weichx.Util {

    public sealed class GameTimer {

        private static int idGenerator;
        private readonly List<Timeout> timers;

        public static readonly GameTimer Instance = new GameTimer();
        
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1);

        private uint frameNumber;

        private GameTimer() {
            timers = new List<Timeout>(16);
        }

        public void Tick() {
            frameNumber++;
            float delta = Time.deltaTime;

            for (int i = 0; i < timers.Count; i++) {
                Timeout timer = timers[i];

                if (timer.Tick(delta)) {
                    timers.RemoveAt(i);
                    i--;
                }
            }
        }

        public float deltaTime {
            get { return Time.deltaTime; }
        }

        public uint frameId {
            get { return frameNumber; }
        }

        public int SetTimeout(float duration, Action callback) {
            Timeout timeout = new Timeout(TimerType.Timeout, callback, duration);
            timers.Add(timeout);
            return timeout.timeoutId;
        }

        public int SetInterval(float interval, Action callback) {
            Timeout timeout = new Timeout(TimerType.Timeout, callback, -1, interval);
            timers.Add(timeout);
            return timeout.timeoutId;
        }

        public int SetInterval(float interval, float duration, Action callback) {
            Timeout timeout = new Timeout(TimerType.TimeoutInterval, callback, duration, interval);
            timers.Add(timeout);
            return timeout.timeoutId;
        }

        public void ClearTimeout(int timerId) {
            for (int i = 0; i < timers.Count; i++) {
                if (timers[i].timeoutId == timerId) {
                    timers.RemoveAt(i);
                    return;
                }
            }
        }

        public float GetFrameTimestamp() {
            return Time.time;
        }

        public float GetRealTimestamp() {
            return Time.realtimeSinceStartup;
        }

        public bool RealTimeElapsed(float duration, float from) {
            return GetRealTimestamp() - from >= duration;
        }

        public bool FrameTimeElapsed(float duration, float from) {
            return GetFrameTimestamp() - from >= duration;
        }


        public static int GetUnixSeconds() {
            return (int) (DateTime.UtcNow.Subtract(Epoch)).TotalSeconds;
        }

        private enum TimerType {

            Timeout,
            Interval,
            TimeoutInterval

        }

        private class Timeout {

            private int ticks;
            public readonly int timeoutId;
            private readonly Action callback;
            private readonly TimerType type;
            private readonly float duration;
            private readonly float interval;
            private float intervalElapsed;
            private float durationElapsed;
            private readonly int maxTicks;

            public Timeout(TimerType type, Action callback, float duration, float interval = 0, int maxTicks = -1) {
                this.type = type;
                this.duration = duration;
                this.interval = interval;
                this.callback = callback;
                this.maxTicks = maxTicks;
                this.ticks = 0;
                this.timeoutId = ++idGenerator;
            }

            public bool Tick(float delta) {
                switch (type) {
                    case TimerType.Timeout:
                        return TickTimeout(delta);
                    case TimerType.Interval:
                        return TickInterval(delta);
                    case TimerType.TimeoutInterval:
                        return TickTimeoutInterval(delta);
                    default: return true;
                }
            }

            private bool TickTimeout(float delta) {
                durationElapsed += delta;

                if (durationElapsed >= duration) {
                    callback();
                    return true;
                }

                return false;
            }

            private bool TickInterval(float delta) {
                intervalElapsed += delta;

                if (intervalElapsed >= interval) {
                    callback();
                    intervalElapsed = 0;
                    ticks++;

                    if (maxTicks > 0 && ticks >= maxTicks) {
                        return true;
                    }
                }

                return false;
            }

            private bool TickTimeoutInterval(float delta) {
                durationElapsed += delta;
                return durationElapsed >= duration || TickInterval(delta);
            }

        }

    }

}