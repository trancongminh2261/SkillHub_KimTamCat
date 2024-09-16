using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project.DTO
{
    public class VideoConfigDetailDTO
    {
        public int LessonVideoId { get; set; }
        public int StopInMinute { get; set; }
        /// <summary>
        /// nhấn tiếp tục
        /// trả lời câu hỏi
        /// </summary>
        public string Type { get; set; }
        public List<VideoConfigQuestionDTO> VideoConfigQuestions { get; set; }
    }
    public class VideoConfigQuestionDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int Index { get; set; }
        public List<VideoConfigOptionDTO> VideoConfigOptions { get; set; }
    }
    public class VideoConfigOptionDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool IsCorrect { get; set; }
    }
}