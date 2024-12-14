using System;

namespace My
{
    internal record class SharedDisposableControlBlock<T>
    {
        public T value { get; set; }
        public int refcount { get; set; }

        public SharedDisposableControlBlock(T value, int refcount) {
            this.value = value;
            this.refcount = refcount;
        }
    }

    /**
        * @note T must be a class just for nullability..
        */
    public struct SharedDisposable<T> : IDisposable
        where T : class, IDisposable
    {
        private SharedDisposableControlBlock<T>? ctrl;

        public SharedDisposable(T? value = null) {
            Reset(value);
        }

        public SharedDisposable(SharedDisposable<T>? copy) {
            Reset(copy?.ctrl, false);
        }

        public void Reset(T? value = null) {
            SharedDisposableControlBlock<T>? rhs = null;
            if (value != null)
                rhs = new(value, refcount: 1);
            Reset(rhs, true);
        }

        private void Reset(SharedDisposableControlBlock<T>? rhs, bool attach) {
            if (ctrl != null) {
                int refcount;
                lock(ctrl) {
                    if (ctrl == rhs)
                        return;
                    if (ctrl.value == rhs?.value)
                        throw new ArgumentException("Same value", nameof(T));
                    refcount = --ctrl.refcount;
                }
                if (refcount == 0) {
                    ctrl.value.Dispose();
                    // ctrl.value = null;
                }
                ctrl = null;
            }

            if (rhs == null)
                return;

            if (!attach) {
                lock (rhs) {
                    if (rhs.refcount == 0)
                        throw new ObjectDisposedException(nameof(T), "Released SharedDisposable");
                    ++rhs.refcount;
                }
            }
            ctrl = rhs;
        }

        public void Reset(SharedDisposable<T> src) {
            Reset(src.ctrl, false);
        }

        public SharedDisposable<T> Share() {
            return new SharedDisposable<T>(this);
        }

        public void Dispose() => Reset((T?)null);

        public bool HasValue { get => ctrl != null; }

        public T? Value { get => ctrl?.value; }
    }
}
