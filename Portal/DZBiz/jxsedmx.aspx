<%@ Page Language="C#" AutoEventWireup="true" CodeFile="jxsedmx.aspx.cs"  Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.jxsedmx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Pragma" content="text/html; charset=utf-8"/>
    <title></title>
    <%--<script src="js/jquery.min.js"></script>
    <link href="WFRes/assets/stylesheets/pixel-admin.min.css" rel="stylesheet" />
    <link href="WFRes/css/MvcSheetTest.css" rel="stylesheet" />--%>
    <script src="DZBiz/jquery.table2excel.min.js"  type="text/javascript"></script>
       <style type="text/css">
           table td{
                border:1px solid #ccc;
           }
    </style>
    <script type="text/javascript">
        // JavaScript Document
        $(document).ready(function (e) {
            debugger
            $("#shxTable").html("");
            $("#sanjiliandong").html("");
            var jxs = '<%= OThinker.H3.Controllers.AppUtility.Engine.Organization.GetUserByCode(this.ActionContext.User.UserCode).Appellation%>';
            if (jxs && jxs != "")
            {
                getjxsed(jxs);
                getjxsedmx(jxs);
            }
            
           
        });
        
        function getjxsed(jxs) {
            $.ajax({
                async: false,
                //url: "ajax/DZBizHandler.ashx",
                url: "/Portal/DZBizHandlerAuth/getjxsed",// 19.6.28 wangxg
                data: { CommandName: "getjxsed", jxs: jxs },
                type: "POST",
                dataType: "json",
                success: function (result) {
                    for (var i = 0; i < result.length; i++) {
                        var tr = "<span>";
                        tr += "【初始额度】：" + result[i].CSFKED + "</td>";
                        tr += "【已用额度】：" + result[i].YSYED + "</td>";
                        tr += "【可用额度】：" + result[i].KYED + "</td>";
                        tr += "</span>";
                        $("#sanjiliandong").html(tr);
                    }
                },
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });
            $("#sanjiliandong").append("<input type=\"button\" onclick=\"dowloadExcel()\" value=\"下载\" style=\"float:right; margin-right: 20px;\"/>")
        }
        function getjxsedmx(jxs) {
            $.ajax({
                async: false,
                //url: "ajax/DZBizHandler.ashx",
                url: "/Portal/DZBizHandlerAuth/getjxsedmx",// 19.6.28 wangxg
                data: { CommandName: "getjxsedmx", jxs: jxs },
                type: "POST",
                dataType: "json",
                success: function (result) {
                    for (var i = 0; i < result.length; i++) {
                        var tr = "<tr>";
                        tr += "<td>" + (i + 1) + "</td>";
                        tr += "<td>" + result[i].KH + "</td>";
                        tr += "<td>" + result[i].HTH + "</td>";
                        tr += "<td>" + result[i].SQBH + "</td>";
                        tr += "<td>" + result[i].DKJE + "</td>";
                        tr += "<td>" + result[i].SYBJ + "</td>";
                        tr += "<td>" + (result[i].FKSJ == null ? "" : result[i].FKSJ) + "</td>";
                        tr += "<td>" + (result[i].FKTS == null ? "" : result[i].FKTS) + "</td>";
                        tr += "</tr>";
                        $("#shxTable").append(tr);

                    }
                },
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });
        }
        //下载Excel

        function dowloadExcel() {
            var filename = "经销商额度信息-" + CurentTime() + ".xls";
            $("#tbResult").table2excel({
                // 导出的Excel文档的名称，（没看到作用）
                name: "Excel Document Name",
                // Excel文件的名称
                filename: filename
            });
        }
        function CurentTime() {
            var now = new Date();

            var year = now.getFullYear();       //年
            var month = now.getMonth() + 1;     //月
            var day = now.getDate();            //日

            //var hh = now.getHours();            //时
            //var mm = now.getMinutes();          //分

            var clock = year + "-";

            if (month < 10)
                clock += "0";

            clock += month + "-";

            if (day < 10)
                clock += "0";

            clock += day + " ";

            //if (hh < 10)
            //    clock += "0";

            //clock += hh + ":";
            //if (mm < 10) clock += '0';
            //clock += mm;
            return (clock);
        }
    </script>

</head>
<body>
    <form id="form1" runat="server"  style="background-color:white;"> 

       <div id="sanjiliandong" style="margin:10px;margin-left:0px;">
            <!--在这里使用三级联动插件-->
        </div>
        <div style="height: auto;  overflow-y: scroll;"  id="tbResult" >
                <table class="table" >
                    <thead>
                        <tr  style="border: 1px solid #ccc;">
                            <td id="" class="rowSerialNo">序号</td>
                            <td id="Td1">
                                <label id="Label26" style="">客户</label>
                            </td>
                            <td id="Td2">
                                <label id="Label109" style="">合同号</label>
                            </td>
                            <td id="Td3">
                                <label id="Label110" style="">申请单号</label>
                            </td>
                              <td id="Td4">
                                <label id="Label1" style="">贷款金额</label>
                            </td>
                            <td id="Td5">
                                <label id="Label2" style="">剩余本金</label>
                            </td>
                              <td id="Td6">
                                <label id="Label3" style="">放款时间</label>
                            </td>
                            <td id="Td7">
                                <label id="Label4" style="">未抵押天数</label>
                            </td>
                        </tr>
                    </thead>
                    <tbody id="shxTable" style="text-align: center;"></tbody>
                </table>
            </div>
    </form>
</body>
</html>
