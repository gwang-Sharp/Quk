using Fisk.EnterpriseManageUtilities.Common;
/*
 * Author: Jerry Bai
 * Create Date: 2011/3/15
 * Description: Generate json string for jQuery tree.
 * Editor: Jerry Bai
 * Update Date: 2013/7/18
 * Licence: BSD
 */
using System;
using System.Collections.Generic;

namespace Fisk.EnterpriseManageUtilities.WebPage.UI.jQuery
{
    /// <summary>
    ///jQueryTreeNode 的摘要说明
    /// </summary>
    [Serializable]
    public class JQueryTreeNode
    {

        public string id { get; set; }
        public string text { get; set; }
        public List<JQueryTreeNode> children
        {
            get;
            set;
        }
        public string iconCls { get; set; }
        public bool checkbox { get; set; }
        public string  state { get; set; }

        public bool ischecked { get; set; }
        public Dictionary<string, object> attributes { get; set; }


        public string  ListToJsonstr(List<JQueryTreeNode> nodes)
        {
           string result= SerializeHelper.JsonSerialize(nodes);
           result = result.Replace("ischecked", "checked");
           return result;
        }


    }

    /// <summary>
    /// 节点状态（打开或关闭）
    /// </summary>
    public enum jQueryTreeNodeState
    {
        /// <summary>
        /// 关闭
        /// </summary>
        closed = 0,

        /// <summary>
        /// 打开
        /// </summary>
        open = 1
    }



    
}