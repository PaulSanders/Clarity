// ****************************************************************************
// <copyright>
// Copyright © Paul Sanders 2014
// </copyright>
// ****************************************************************************
// <author>Paul Sanders</author>
// <project>Clarity</project>
// <web>http://clarity.codeplex.com</web>
// <license>
// See license.txt in this solution
// </license>
// ****************************************************************************
using System.Collections.ObjectModel;
using Clarity.Commands;

namespace Clarity.Winforms
{
    public class GetAnswerViewModel : ViewModel
    {
        private string _message;
        public virtual string Message
        {
            get
            {
                return _message;
            }
            set
            {
                SetValue(ref _message, value, () => Message);
            }
        }

        private ObservableCollection<WindowResult> _answers = new ObservableCollection<WindowResult>();
        public virtual ObservableCollection<WindowResult> Answers
        {
            get
            {
                return _answers;
            }
            set
            {
                SetValue(ref _answers, value, () => Answers);
            }
        }

        #region ChooseAnswer Command
        private IClarityCommand _chooseAnswer;
        public IClarityCommand ChooseAnswer
        {
            get
            {
                if (_chooseAnswer == null)
                {
                    _chooseAnswer = ServiceManager.Default.Resolve<ICommandBuilder>().BuildDelegate<WindowResult>(ExecuteChooseAnswer);
                }
                return _chooseAnswer;
            }
        }

        private void ExecuteChooseAnswer(WindowResult answer)
        {
            SelectedAnswer = answer;
        }
        #endregion

        private WindowResult _selectedAnswer;
        public virtual WindowResult SelectedAnswer
        {
            get
            {
                return _selectedAnswer;
            }
            set
            {
                SetValue(ref _selectedAnswer, value, () => SelectedAnswer, () => Close());
            }
        }

        public override bool CanClose()
        {
            return SelectedAnswer != null;
        }
    }
}
