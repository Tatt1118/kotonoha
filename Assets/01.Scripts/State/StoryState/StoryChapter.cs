using System.Collections.Generic;

public class StoryChapter
{
    public string ChapterId { get; }
    public List<StoryData> DataList { get; }
    public bool RequiresPhoneInput { get; }  
    public StoryChapter(string chapterId, List<StoryData> dataList, bool requiresPhoneInput = false)
    {
        ChapterId = chapterId;
        DataList = dataList;
        RequiresPhoneInput = requiresPhoneInput;
    }
}
