﻿using System;
using Zenject;

namespace HttpSiraStatus.Models
{
    public class CustomCutBuffer : CutScoreBuffer
    {
        public NoteCutInfo NoteCutInfo { get; private set; }
        public ICutScoreBufferDidFinishEvent FinishEvent { get; set; }
        public NoteController NoteController { get; private set; }

        public void Init(in NoteCutInfo noteCutInfo, int multiplier, NoteController controller, ICutScoreBufferDidFinishEvent e)
        {
            this.FinishEvent = e;
            this.NoteController = controller;
            this.didFinishEvent.Add(this.FinishEvent);
            this.NoteCutInfo = noteCutInfo;
            base.Init(noteCutInfo, multiplier);
        }
        public void Reflesh()
        {
            this.didFinishEvent.Remove(this.FinishEvent);
            this.NoteController = null;
            this.FinishEvent = null;
        }
        public new class Pool : MemoryPool<NoteCutInfo, int, NoteController, ICutScoreBufferDidFinishEvent, CustomCutBuffer>, IDisposable
        {
            // GCに勝手に回収されない用
            private readonly LazyCopyHashSet<CustomCutBuffer> activeItems = new LazyCopyHashSet<CustomCutBuffer>(256);
            private bool disposeValue = false;

            protected override void OnSpawned(CustomCutBuffer item)
            {
                this.activeItems.Add(item);
            }

            protected override void Reinitialize(NoteCutInfo p1, int p2, NoteController p3, ICutScoreBufferDidFinishEvent p4, CustomCutBuffer item) => item.Init(p1, p2, p3, p4);
            protected override void OnDespawned(CustomCutBuffer item)
            {
                item.Reflesh();
                this.activeItems.Remove(item);
            }
            protected override void OnDestroyed(CustomCutBuffer item)
            {
                item.Reflesh();
                this.activeItems.Remove(item);
            }

            protected virtual void Dispose(bool disposing)
            {
                Plugin.Logger.Debug("Dispose()");
                if (!this.disposeValue) {
                    if (disposing) {
                        foreach (var buff in this.activeItems.items) {
                            buff.Reflesh();
                        }
                        this.activeItems.items.Clear();
                    }
                    this.disposeValue = true;
                }
            }

            public new void Dispose()
            {
                base.Dispose();
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }
        }
    }
}
