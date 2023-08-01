using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models.ManageViewModels
{
    public class RequestStepViewModel
    {
        public int RequestTypeId { get; set; }
        public int Version { get; set; }
        public int StepId { get; set; }
        public int OldStepId { get; set; }
        public string StepSlug { get; set; }
        public string StepName { get; set; }
        public string StepDescription { get; set; }
        public int StepTypeId { get; set; }
        public string StepTypeName { get; set; }
        public string StepTypeColor { get; set; }
        public bool StepApplyApprovalPolicyForAssignees { get; set; }
        public List<WorkFlowStepTypeButtonViewModel> StepsTypeButtons { get; set; }
        public bool IsConditional { get; set; }
        public bool IsScheduled { get; set; }
        public int TAT { get; set; }
        public string TATInDays 
        { 
            get
            {
                int days = TAT / 480;
                int hours = (TAT % 480) / 60;
                int mins = TAT % 60;

                return days.ToString() + " day(s), " + hours.ToString() + " hour(s), " + mins.ToString() + " min(s)";
            }
        }
        public bool EnableEmail { get; set; }
        public bool EnableSMS { get; set; }
        public int Sequence { get; set; }
        public bool InvolvedInWorkFlows { get; set; }
        public bool IsMapToConditions { get; set; }
        public RequestStepConditionViewModel Condition { get; set; }
        public List<RequestStepAssigneeViewModel> Assignees { get; set; }
        public WorkFlowStepViewModel WorkFlowStep { get; set; }
        public RequestTypeViewModel RequestType { get; set; }
        public List<RequestTypeWorkFlowViewModel> WorkFlows { get; set; }
        public List<RequestTypeWorkFlowViewModel> NextWorkFlows { get; set; }

        //Temporary properties
        public int Id { get; set; }
        public string Name { get; set; }
        
    }
}
