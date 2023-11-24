using System;

namespace My
{
    class SharedDisposableControlBlock<T>
    {
        public T? value;
        public int refcount;
    }

    public struct SharedDisposable<T> : IDisposable
        where T : class, IDisposable
    {
        private SharedDisposableControlBlock<T>? ctrl;

        public SharedDisposable(T? value = null) {
            Reset(value);
        }

        public SharedDisposable(SharedDisposable<T>? copy) {
            Reset(copy?.ctrl);
        }

        public void Reset(T? value = null) {
            if (ctrl != null) {
                int refcount;
                lock(ctrl) {
                    if (ctrl.value == value)
                        throw new ArgumentException("Same value", nameof(T));
                    refcount = --ctrl.refcount;
                }
                if (refcount == 0) {
                    Value.Dispose();
                    // ctrl.value = null;
                }
                ctrl = null;
            }

            if (value == null)
                return;

            ctrl = new() {
                value = value,
                refcount = 1,
            };
        }

        private void Reset(SharedDisposableControlBlock<T>? rhs) {
            if (ctrl != null) {
                int refcount;
                lock(ctrl) {
                    if (ctrl == rhs)
                        return;
                    refcount = --ctrl.refcount;
                }
                if (refcount == 0) {
                    Value.Dispose();
                    // ctrl.value = null;
                }
                ctrl = null;
            }

            if (rhs == null)
                return;

            lock(rhs) {
                if (rhs.refcount == 0)
                    throw new ObjectDisposedException(nameof(T), "Released SharedDisposable");
                ++rhs.refcount;
            }
            ctrl = rhs;
        }

        public void Reset(SharedDisposable<T> src) {
            Reset(src.ctrl);
        }

        public SharedDisposable<T> Share() {
            var copy = new SharedDisposable<T>();
            copy.Reset(ctrl);
            return copy;
        }

        public void Dispose() => Reset((T?)null);

        public bool HasValue { get => ctrl != null; }

        public T? Value { get => ctrl?.value!; }
    }
}
