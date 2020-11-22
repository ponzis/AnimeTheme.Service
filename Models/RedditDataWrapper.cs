using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace AnimeTheme.Service.Models
{
    
    public class MdConverter : JsonConverter<string>
    {
        public override string Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            var value = reader.GetString();
            return Regex.Unescape(value ?? string.Empty);

        }

        public override void Write(
            Utf8JsonWriter writer,
            string value,
            JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
    
    public class UnixTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }
            var result = DateTime.UnixEpoch.AddSeconds(reader.GetDouble());
            return result;
        }

        public override void Write(
            Utf8JsonWriter writer,
            DateTime dateTimeValue,
            JsonSerializerOptions options)
        {
            var seconds = (long)(dateTimeValue.ToUniversalTime() - DateTime.UnixEpoch).TotalSeconds;
            writer.WriteNumberValue(seconds);
        }
    }
    
    public class RedditDataWrapper
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum RedditDataType
        {
            wikipage, t2
        
        }
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum SubredditType
        {
            user,
        }
        
        
        
        public class RedditDataModel 
        {
            [JsonPropertyName("content_md")] 
            [JsonConverter(typeof(MdConverter))]
            public string ContentMd { get; init; }
            [JsonPropertyName("may_revise")] public bool MayRevise { get; init; }
            [JsonPropertyName("reason")] public string Reason { get; init; }
            
            [JsonPropertyName("revision_date")] 
            [JsonConverter(typeof(UnixTimeConverter))]
            public DateTime RevisionDate { get; init; }
            
            [JsonPropertyName("revision_by")] public RedditDataWrapper RevisionBy { get; init; }
            [JsonPropertyName("is_employee")] public bool IsEmployee { get; init; }
            [JsonPropertyName("icon_img")] public Uri IconImg { get; init; }
            [JsonPropertyName("pref_show_snoovatar")] public bool PrefShowSnoovatar { get; init; }
            [JsonPropertyName("name")] public string Name { get; init; }
            [JsonPropertyName("is_friend")] public bool IsFriend { get; init; }
            [JsonConverter(typeof(UnixTimeConverter))]
            [JsonPropertyName("created")] public DateTime Created { get; init; }
            [JsonPropertyName("has_subscribed")] public bool HasSubscribed { get; init; }
            [JsonPropertyName("hide_from_robots")] public bool HideFromRobots { get; init; }
            [JsonPropertyName("is_mod")] public bool IsMod { get; init; }
            [JsonConverter(typeof(UnixTimeConverter))]
            [JsonPropertyName("created_utc")] public DateTime CreatedUtc { get; init; }
            [JsonPropertyName("snoovatar_img")] public Uri SnoovatarImg { get; init; }
            [JsonPropertyName("subreddit")] public RedditSubredditDataModel Subreddit { get; init; }
        }
        
        public class RedditSubredditDataModel
        {
            [JsonPropertyName("default_set")] public bool DefaultSet { get; init; }
            [JsonPropertyName("user_is_contributor")] public bool? UserIsContributor { get; init; }
            [JsonPropertyName("banner_img")] public Uri BannerImg { get; init; }
            [JsonPropertyName("restrict_posting")] public bool RestrictPosting { get; init; }
            [JsonPropertyName("user_is_banned")] public bool? UserIsBanned { get; init; }
            [JsonPropertyName("free_form_reports")] public bool FreeFormReports { get; init; }
            [JsonPropertyName("community_icon")] public Uri CommunityIcon { get; init; }
            [JsonPropertyName("show_media")] public bool ShowMedia { get; init; }
            [JsonPropertyName("icon_color")] public string IconColor { get; init; }
            [JsonPropertyName("user_is_muted")] public bool? UserIsMuted { get; init; }
            [JsonPropertyName("display_name")] public string DisplayName { get; init; }
            [JsonPropertyName("header_img")] public Uri HeaderImg { get; init; }
            [JsonPropertyName("title")] public string Title { get; init; }
            [JsonPropertyName("previous_names")] public string[] PreviousNames { get; init; }
            [JsonPropertyName("over_18")] public bool Over18 { get; init; }
            [JsonPropertyName("icon_size")] public int[] IconSize { get; init; }
            [JsonPropertyName("primary_color")] public string PrimaryColor { get; init; }
            [JsonPropertyName("icon_img")] public Uri IconImg { get; init; }
            [JsonPropertyName("description")] public string Description { get; init; }
            [JsonPropertyName("submit_link_label")] public string SubmitLinkLabel { get; init; }
            [JsonPropertyName("header_size")] public int? HeaderSize { get; init; }
            [JsonPropertyName("restrict_commenting")] public bool RestrictCommenting { get; init; }
            [JsonPropertyName("subscribers")] public int Subscribers { get; init; }
            [JsonPropertyName("submit_text_label")] public string SubmitTextLabel { get; init; }
            [JsonPropertyName("is_default_icon")] public bool IsDefaultIcon { get; init; }
            [JsonPropertyName("link_flair_position")] public string LinkFlairPosition { get; init; }
            [JsonPropertyName("display_name_prefixed")] public string DisplayNamePrefixed { get; init; }
            [JsonPropertyName("key_color")] public string KeyColor { get; init; }
            [JsonPropertyName("name")] public string Name { get; init; }
            [JsonPropertyName("is_default_banner")] public bool IsDefaultBanner { get; init; }
            [JsonPropertyName("url")] public Uri Url { get; init; }
            [JsonPropertyName("quarantine")] public bool Quarantine { get; init; }
            [JsonPropertyName("banner_size")] public int[] BannerSize { get; init; }
            [JsonPropertyName("user_is_moderator")] public bool? UserIsModerator { get; init; }
            [JsonPropertyName("public_description")] public string PublicDescription { get; init; }
            [JsonPropertyName("link_flair_enabled")] public bool LinkFlairEnabled { get; init; }
            [JsonPropertyName("disable_contributor_requests")] public bool DisableContributorRequests { get; init; }
            [JsonPropertyName("subreddit_type")] public SubredditType SubredditType { get; init; }
            [JsonPropertyName("user_is_subscriber")] public bool? UserIsSubscriber { get; init; }
        }
        
        [EnumDataType(typeof(RedditDataType))]
        [JsonPropertyName("kind")]
        public RedditDataType Kind { get; init; }
        
        [JsonPropertyName("data")]
        public RedditDataModel Data { get; init; }
        
        [JsonPropertyName("revision_id")]
        public Guid RevisionId { get; init; }
        
        [JsonPropertyName("content_html")]
        public string ContentHtml { get; init; }
        
    }
}