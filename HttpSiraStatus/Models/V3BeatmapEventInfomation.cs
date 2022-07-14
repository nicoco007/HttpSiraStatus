﻿using HttpSiraStatus.Interfaces;
using HttpSiraStatus.Util;
using Zenject;

namespace HttpSiraStatus.Models
{
    internal class V3BeatmapEventInfomation : IBeatmapEventInformation
    {
        public string version { get; } = "3.0.0";
        public V3BeatmapEventType BeatmapEventType { get; private set; }
        #region BPM
        public float BPM { get; private set; }
        #endregion

        #region ColorBoost
        public bool BoostColorsAreOn { get; private set; }
        #endregion

        #region LightCommon
        public int GroupId { get; private set; } = -1;
        public int ElementId { get; private set; } = -1;
        #endregion

        #region LightColor
        public BeatmapEventTransitionType TransitionType { get; private set; }
        public EnvironmentColorType ColorType { get; private set; }
        public float Brightness { get; private set; }
        public int StrobeBeatFrequency { get; private set; }
        #endregion

        #region LightRotation
        public bool UsePreviousEventValue { get; private set; }
        public EaseType EaseType { get; private set; }
        public LightRotationBeatmapEventData.Axis Axis { get; private set; }
        public int LoopCount { get; private set; }
        public LightRotationDirection RotationDirection { get; private set; }
        public float Rotation { get; private set; }
        #endregion

        public void Init(BeatmapEventData eventData)
        {
            switch (eventData) {
                case BPMChangeBeatmapEventData bpm:
                    this.BeatmapEventType = V3BeatmapEventType.BPM;
                    this.BPM = bpm.bpm;
                    break;
                case ColorBoostBeatmapEventData color:
                    this.BeatmapEventType = V3BeatmapEventType.ColorBoost;
                    this.BoostColorsAreOn = color.boostColorsAreOn;
                    break;
                case LightColorBeatmapEventData lightColor:
                    this.BeatmapEventType = V3BeatmapEventType.LightColor;
                    this.GroupId = lightColor.groupId;
                    this.ElementId = lightColor.elementId;
                    this.TransitionType = lightColor.transitionType;
                    this.ColorType = lightColor.colorType;
                    this.Brightness = lightColor.brightness;
                    this.StrobeBeatFrequency = lightColor.strobeBeatFrequency;
                    break;
                case LightRotationBeatmapEventData lightRotation:
                    this.BeatmapEventType = V3BeatmapEventType.LightRotation;
                    this.GroupId = lightRotation.groupId;
                    this.ElementId = lightRotation.elementId;
                    this.UsePreviousEventValue = lightRotation.usePreviousEventValue;
                    this.EaseType = lightRotation.easeType;
                    this.Axis = lightRotation.axis;
                    this.LoopCount = lightRotation.loopCount;
                    this.RotationDirection = lightRotation.rotationDirection;
                    this.Rotation = lightRotation.rotation;
                    break;
                case SpawnRotationBeatmapEventData spawn:
                    this.BeatmapEventType = V3BeatmapEventType.SpawnRotation;
                    this.Rotation = spawn.rotation;
                    break;
                case BasicBeatmapEventData basic:
                default:
                    this.BeatmapEventType = V3BeatmapEventType.Unknown;
                    break;
            }
        }

        public void Reset()
        {
            this.BeatmapEventType = V3BeatmapEventType.Unknown;
            this.BPM = 0;
            this.BoostColorsAreOn = false;
            this.GroupId = -1;
            this.ElementId = -1;
            this.TransitionType = BeatmapEventTransitionType.Instant;
            this.ColorType = EnvironmentColorType.Color0;
            this.Brightness = 0;
            this.StrobeBeatFrequency = 0;
            this.UsePreviousEventValue = false;
            this.EaseType = EaseType.None;
            this.Axis = LightRotationBeatmapEventData.Axis.X;
            this.LoopCount = 0;
            this.RotationDirection = LightRotationDirection.Automatic;
            this.RotationDirection = 0;
        }

        public JSONObject ToJson()
        {
            var result = new JSONObject();
            result["version"] = this.version;
            result["type"] = (int)this.BeatmapEventType;
            switch (this.BeatmapEventType) {
                case V3BeatmapEventType.BPM:
                    result["bpm"] = this.BPM;
                    break;
                case V3BeatmapEventType.ColorBoost:
                    result["boostColorsAreOn"] = this.BoostColorsAreOn;
                    break;
                case V3BeatmapEventType.LightColor:
                    result["groupId"] = this.GroupId;
                    result["elementId"] = this.ElementId;
                    result["transitionType"] = $"{this.TransitionType}";
                    result["colorType"] = $"{this.ColorType}";
                    result["brightness"] = this.Brightness;
                    result["strobeBeatFrequency"] = this.StrobeBeatFrequency;
                    break;
                case V3BeatmapEventType.LightRotation:
                    result["groupId"] = this.GroupId;
                    result["elementId"] = this.ElementId;
                    result["usePreviousEventValue"] = this.UsePreviousEventValue;
                    result["easeType"] = $"{this.EaseType}";
                    result["axis"] = $"{this.Axis}";
                    result["loopCount"] = this.LoopCount;
                    result["rotationDirection"] = $"{this.RotationDirection}";
                    result["rotation"] = this.Rotation;
                    break;
                case V3BeatmapEventType.SpawnRotation:
                    result["rotation"] = this.Rotation;
                    break;
                case V3BeatmapEventType.Unknown:
                default:
                    break;
            }
            return result;
        }

        public class Pool : MemoryPool<V3BeatmapEventInfomation>
        {
            protected override void OnDespawned(V3BeatmapEventInfomation item)
            {
                item.Reset();
            }
        }
    }
}