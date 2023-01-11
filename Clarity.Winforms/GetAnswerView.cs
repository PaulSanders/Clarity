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
using System.Windows.Forms;

namespace Clarity.Winforms
{
    public partial class GetAnswerView : View
    {
        public GetAnswerView()
        {
            InitializeComponent();
        }

        public override void BindViewModel()
        {
            if (ParentWindow == null) return;

            BindControlProperty(lblMessage, "Message");

            pnl.ColumnCount = ViewModel.Answers.Count;
            var pct = 100 / pnl.ColumnCount;

            int i = 0;
            pnl.ColumnStyles.Clear();

            for (; i < pnl.ColumnCount; i++)
            {
                pnl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, pct));
            }

            i = 0;
            foreach (var result in ViewModel.Answers)
            {
                var button = CreateButton(result);
                pnl.Controls.Add(button, i++, 0);

                if (result.IsCancel)
                    ParentWindow.CancelButton = button;
                else if (result.IsDefault)
                    ParentWindow.AcceptButton = button;
            }
        }

        private Button CreateButton(WindowResult result)
        {
            var btn = new Button();
            btn.Text = result.Text;
            btn.Margin = new Padding(2);
            btn.Width = 100;
            btn.MaximumSize = new System.Drawing.Size(100, 30);
            btn.Dock = DockStyle.Fill;
            btn.Anchor = AnchorStyles.None;

            BindButton(btn, ViewModel.ChooseAnswer, result);

            return btn;
        }

        public new GetAnswerViewModel ViewModel
        {
            get
            {
                return (GetAnswerViewModel)base.ViewModel;
            }
        }
    }
}
