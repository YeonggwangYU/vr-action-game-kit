using System;
using UnityEngine;
using UnityEngine.Playables;

namespace _3DGamekitLite.Scripts.Runtime.Game.Timeline.TimeMachine
{
	[Serializable]
	public class TimeMachineBehaviour : PlayableBehaviour
	{
		public TimeMachineAction action;
		public Condition condition;
		public string markerToJumpTo, markerLabel;
		public float timeToJumpTo;

		[HideInInspector]
		public bool clipExecuted = false; //the user shouldn't author this, the Mixer does

		public bool ConditionMet()
		{
			switch(condition)
			{
				case Condition.Always:
					return true;

				case Condition.Never:
				default:
					return false;
			}
		}

		public enum TimeMachineAction
		{
			Marker,
			JumpToTime,
			JumpToMarker,
			Pause,
		}

		public enum Condition
		{
			Always,
			Never,
		}
	}
}
