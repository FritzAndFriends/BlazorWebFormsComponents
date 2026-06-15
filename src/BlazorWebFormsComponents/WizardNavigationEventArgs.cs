using System;

namespace BlazorWebFormsComponents
{

	public class WizardNavigationEventArgs : EventArgs
	{

		public WizardNavigationEventArgs(int currentStepIndex, int nextStepIndex)
		{
			CurrentStepIndex = currentStepIndex;
			NextStepIndex = nextStepIndex;
		}

		public bool Cancel { get; set; }

		public int CurrentStepIndex { get; }

		public int NextStepIndex { get; }

	}

}
