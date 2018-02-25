using Newtonsoft.Json;
using Nfield.ApiTool.Helper;
using Nfield.ApiTool.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Nfield.ApiTool.ViewModels
{
    public class SurveyStatisticsViewModel : INotifyPropertyChanged
    {
        private SurveyDetails SurveyDetails { get; set; }

        public ObservableCollection<SurveyInfo> _surveyInfo { get; set; }
        public ObservableCollection<SurveyInfo> SurveyInfo
        {
            get { return _surveyInfo; }
            set
            {
                if (_surveyInfo != value)
                {
                    _surveyInfo = value;
                    OnPropertyChanged("SurveyInfo");
                }
            }
        }

        public ObservableCollection<QuotaGroup> _quotaGroup { get; set; }
        public ObservableCollection<QuotaGroup> QuotaGroup
        {
            get { return _quotaGroup; }
            set
            {
                if (_quotaGroup != value)
                {
                    _quotaGroup = value;
                    OnPropertyChanged("QuotaGroup");
                }
            }
        }
        private SurveyCountsModel _surveyCountsModel;
        public SurveyCountsModel SurveyCounts
        {
            get { return _surveyCountsModel; }
            set
            {
                if (_surveyCountsModel != value)
                {
                    _surveyCountsModel = value;
                    OnPropertyChanged("SurveyCounts");
                }
            }
        }

        public bool HasQuota { get; set; }
        public SurveyStatisticsViewModel(AccessToken token, string serverUrl, SurveyDetails surveyDetails)
        {
            Task.Run(async () => await RetrieveCounts(token, serverUrl, surveyDetails));
        }

        private async Task RetrieveCounts(AccessToken token, string serverUrl, SurveyDetails surveyDetails)
        {
            var url = $"{serverUrl}/v1/Surveys/{surveyDetails.SurveyId}/Counts";
            var request = new RestApi().Get(url, token);

            using (WebResponse response = await request.GetResponseAsync())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = reader.ReadToEnd();
                    SurveyCounts = JsonConvert.DeserializeObject<SurveyCountsModel>(content);
                }
            }

            Percentages(surveyDetails);
        }

        private void Percentages(SurveyDetails surveyDetails)
        {
            try
            {
                var total = (SurveyCounts.SuccessfulCount
               + SurveyCounts.DroppedOutCount
               + SurveyCounts.ScreenedOutCount
               + SurveyCounts.RejectedCount);

                if (total == null) total = 0;
                var successPerc = total != 0 ? Math.Round((((decimal)SurveyCounts.SuccessfulCount / (decimal)total) * 100), 1) : 0;
                var dropPerc = total != 0 ? Math.Round((((decimal)SurveyCounts.DroppedOutCount / (decimal)total) * 100), 1) : 0;
                var screenPerc = total != 0 ? Math.Round((((decimal)SurveyCounts.ScreenedOutCount / (decimal)total) * 100), 1) : 0;
                var rejectPerc = total != 0 ? Math.Round((((decimal)SurveyCounts.RejectedCount / (decimal)total) * 100), 1) : 0;
                var totalPerc = total != 0 ? Math.Round((successPerc + dropPerc + screenPerc + rejectPerc), 1) : 0;

                SurveyInfo = new ObservableCollection<SurveyInfo>
                {
                    new SurveyInfo
                    {
                        SurveyName = surveyDetails.SurveyName,
                        Success = $"{SurveyCounts.SuccessfulCount} total successful interviews",
                        ActiveLive = $"{SurveyCounts.ActiveLiveCount} active live interviews",
                        ActiveTest = $"{SurveyCounts.ActiveTestCount} active test interviews",
                        Total = (SurveyCounts.SuccessfulCount
                        + SurveyCounts.DroppedOutCount
                        + SurveyCounts.ScreenedOutCount
                        + SurveyCounts.RejectedCount).ToString(),
                        PercSuccess = $"{successPerc}%",
                        PercDrop = $"{dropPerc}%",
                        PercScreen = $"{screenPerc}%",
                        PercReject = $"{rejectPerc}%",
                        PercTotal = $"{totalPerc}%",
                        SurveyCounts = SurveyCounts
                    }
                };

                if (SurveyCounts.QuotaCounts != null)
                {
                    SurveyInfo[0].HasNoQuota = false;
                    SurveyInfo[0].TargetVisible = true;
                    var targetPercentage = Math.Round((((decimal)SurveyCounts.SuccessfulCount / (decimal)SurveyCounts.QuotaCounts.Target) * 100), 1);
                    SurveyInfo[0].PercOfTarget = $"{targetPercentage}% of Target";
                    SurveyInfo[0].Target = SurveyCounts.QuotaCounts.Target.ToString();
                    SetUpQuotas();
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        private void SetUpQuotas()
        {
            try
            {
                QuotaGroup = new ObservableCollection<QuotaGroup>();
                var group = new QuotaGroup("");
                QuotaGroup = QuotaGroups(SurveyCounts.QuotaCounts.Attributes, group, QuotaGroup, "");

            }
            catch (Exception)
            {
                throw;
            }
        }

        private ObservableCollection<QuotaGroup> QuotaGroups(List<QuotaAttribute> quotaAttributes,
            QuotaGroup group,
            ObservableCollection<QuotaGroup> quotaCollection,
            string name,
            string levelName = "")
        {
            foreach (var attribute in quotaAttributes)
            {
                name = string.IsNullOrEmpty(levelName) ? attribute.Name : levelName;

                group = new QuotaGroup(name);
                foreach (var level in attribute.Levels)
                {
                    group.Add(level);

                    QuotaGroups(level.Attributes, group, quotaCollection, name, level.Name);
                }

                quotaCollection.Add(group);
            }

            return quotaCollection;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
