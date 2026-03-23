using System;
using System.Net;
using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

namespace AfterDepartmentPortal.Components.Controls
{
    public class PollQuestion : WebControl
    {
        [Parameter]
        public string QuestionText { get; set; } = string.Empty;

        [Parameter]
        public string Options { get; set; } = string.Empty;

        [Parameter]
        public int SelectedOption { get; set; } = -1;

        [Parameter]
        public EventCallback<PollVoteEventArgs> VoteSubmitted { get; set; }

        protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "poll-question");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "poll-question-text");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.Write(WebUtility.HtmlEncode(QuestionText));
            writer.RenderEndTag();

            var optionArray = Options.Split(',');
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "poll-options");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            var controlId = !string.IsNullOrEmpty(ID) ? ClientID : "poll";

            for (var i = 0; i < optionArray.Length; i++)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "poll-option");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                writer.AddAttribute(HtmlTextWriterAttribute.Type, "radio");
                writer.AddAttribute(HtmlTextWriterAttribute.Name, controlId + "_option");
                writer.AddAttribute(HtmlTextWriterAttribute.Value, i.ToString());
                writer.AddAttribute(HtmlTextWriterAttribute.Id, controlId + "_option_" + i);
                if (SelectedOption == i)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked");
                }
                writer.RenderBeginTag(HtmlTextWriterTag.Input);
                writer.RenderEndTag();

                writer.AddAttribute(HtmlTextWriterAttribute.For, controlId + "_option_" + i);
                writer.RenderBeginTag(HtmlTextWriterTag.Label);
                writer.Write(WebUtility.HtmlEncode(optionArray[i].Trim()));
                writer.RenderEndTag();

                writer.RenderEndTag(); // poll-option div
            }

            writer.RenderEndTag(); // poll-options div

            writer.AddAttribute(HtmlTextWriterAttribute.Type, "button");
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "poll-submit-button");
            writer.RenderBeginTag(HtmlTextWriterTag.Button);
            writer.Write("Vote");
            writer.RenderEndTag();

            writer.RenderEndTag(); // poll-question div
        }
    }
}
