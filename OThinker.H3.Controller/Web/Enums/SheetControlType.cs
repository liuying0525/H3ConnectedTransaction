using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// SheetControl类别
    /// </summary>
    public enum SheetContorlType
    {
        /// <summary>
        /// SheetActonPanel
        /// </summary>
        SheetActonPanel = -3,
        /// <summary>
        /// 
        /// </summary>
        Label = -2,
        /// <summary>
        /// 
        /// </summary>
        SheetLabel = -1,
        /// <summary>
        /// 
        /// </summary>
        SheetTextBox = 2,
        /// <summary>
        /// 
        /// </summary>
        SheetDropDownList = 3,
        /// <summary>
        /// 
        /// </summary>
        SheetRadioButtonList = 4,
        /// <summary>
        /// 
        /// </summary>
        SheetBizDropDownList = 5,
        /// <summary>
        /// 
        /// </summary>
        SheetCheckBox = 6,
        /// <summary>
        /// 
        /// </summary>
        SheetCheckBoxList = 7,
        /// <summary>
        /// 
        /// </summary>
        SheetAttachment = 8,
        /// <summary>
        /// 
        /// </summary>
        SheetAttachmentNTKO = 9,
        /// <summary>
        /// 
        /// </summary>
        SheetTime = 10,
        /// <summary>
        /// 
        /// </summary>
        SheetRichTextBox = 11,
        /// <summary>
        /// 
        /// </summary>
        SheetComment = 12,
        /// <summary>
        /// 
        /// </summary>
        SheetDetail = 13,
        /// <summary>
        /// 
        /// </summary>
        SheetGridView = 14,
        /// <summary>
        /// 
        /// </summary>
        SheetBizDetail = 15,
        /// <summary>
        /// 
        /// </summary>
        SheetSingleUserSelector = 16,
        /// <summary>
        /// 
        /// </summary>
        SheetMultiUserSelector = 17,
        /// <summary>
        /// 
        /// </summary>
        SheetUserList = 18
    }
}