//选人控件
(function ($) {
    // 控件执行
    // 参数{AutoTrim:true,DefaultValue:datavalue,OnChange:""}
    //可以通过  $("#id").SheetTextBox(参数)  来渲染控件和获取控件对象
    $.fn.SheetUser = function () {
        return $.MvcSheetUI.Run.call(this, "SheetUser", arguments);
    };

    // 构造函数
    $.MvcSheetUI.Controls.SheetUser = function (element, ptions, sheetInfo) {
        // 选择数据集合
        this.Choices = {};
        // 所有选择的元素
        this.ChoicesElement = null;
        // 搜索输入框元素
        this.SearchElement = null;
        this.SearchTxtElement = null;
        this.SearchButton = null;
        // 获取选中的组织ID
        this.SelectedValue = null;
        // 获取当前搜索关键字
        this.SearchKey = null;
        this.SearchMode = false;
        this.KeyTime = null;
        // 搜索结果
        this.SearchResults = [];
        // 组织机构容器
        this.SelectorPanel = null;
        // 只在 Enter 进行搜索
        this.EnterSearch = true;
        this.OrgTreePanel = null;
        this.OrgListPanel = null;
        this.IsOverSelectorPanel = false;
        this.SheetUserHandler = $.MvcSheetUI.PortalRoot + "/SheetUser/LoadOrgTreeNodes";
        this.SheetGetUserProperty = $.MvcSheetUI.PortalRoot + "/SheetUser/GetUserProperty";
        this.ModelControl = null;

        $.MvcSheetUI.Controls.SheetUser.Base.constructor.call(this, element, ptions, sheetInfo);
    };

    // 继承及控件实现
    $.MvcSheetUI.Controls.SheetUser.Inherit($.MvcSheetUI.IControl, {
        //移动端
        RenderMobile: function () {
            //baosc s
            //初始展示当前用户OU人员  包括虚拟用户
            //if ($.MvcSheetUI.SheetInfo.UserOUMembers || !this.UserVisible) {
            //} else {
            //    $.MvcSheetUI.SheetInfo.UserOUMembers = [];
            //    var parentUnits = $.MvcSheetUI.SheetInfo.DirectParentUnits;
            //    for (var key in parentUnits) {
            //        var loadUrl = this.SheetUserHandler + "?IsMobile=true&ParentID=" + key + "&o=U";
            //        $.ajax({
            //            type: "GET",
            //            url: loadUrl,
            //            dataType: "json",
            //            async: false,//同步执行
            //            success: function (data) {
            //                var filterdata = $.grep(data, function (value) {
            //                    if (value.ExtendObject.UnitType == "U") {
            //                        return value;
            //                    }
            //                });
            //                $.MvcSheetUI.SheetInfo.UserOUMembers = $.MvcSheetUI.SheetInfo.UserOUMembers.concat(filterdata);
            //            }
            //        });
            //    }
            //}
            ////baosc e

            //是否多选
            this.IsMultiple = this.LogicType == $.MvcSheetUI.LogicType.MultiParticipant;
            //不可用
            if (!this.Editable) {
                $(this.Element).prop("readonly", true);
                $(this.Element).addClass(this.Css.Readonly);
                $(this.Element).addClass("item-content");
            } else {
                this.MoveToMobileContainer();
                var that = this;
                //ionic初始化控件
                this.ionicInit(that, $.MvcSheetUI.IonicFramework);
            }
            //初始化默认值
            this.InitValue();
        },
        //Ionic初始化
        ionicInit: function (that, ionic) {
            var loadOptions = that.GetLoadTreeOption();
            var loadUrl = that.SheetUserHandler + "?IsMobile=true&Recursive=" + that.Recursive;
            var ngmodel = that.DataField + that.Options.RowNum;
            that.Mask.parent().addClass("item-icon-right");
            that.Mask.after('<i class="icon ion-ios-arrow-right"></i>');
            that.Mask.parent().parent().attr("data-scopedata", that.DataField);
            that.Mask.parent().parent().attr("data-loadurl", loadUrl);
            that.Mask.parent().parent().attr("data-datafield", that.DataField);
            that.Mask.parent().parent().unbind("click.sheetUser").bind("click.sheetUser", function () {
                var dataField = $(this).attr("data-scopedata");
                var loadUrl = $(this).attr("data-loadurl");
                var obj = ionic.$scope[ngmodel];
                var options = {
                    options: obj.Options,
                    selecFlag: that.GetSelectableFlag(),
                    dataField: dataField,
                    ngmodel: ngmodel,
                    loadUrl: loadUrl,
                    loadOptions: obj.GetLoadTreeOption(),
                    initUsers: obj.GetChoices(),
                    isMutiple: obj.IsMultiple
                };
                $.MvcSheetUI.sheetUserParams = options;
                ionic.$state.go("form.sheetuser", { index: 0, parentID: "" });
            });
            ionic.$scope[ngmodel] = that;
        },
        //PC端
        Render: function () {
            if (!this.Visiable) {
                this.Element.style.display = "none";
                return;
            }
            // 查看痕迹
            if (this.TrackVisiable && !this.Originate && this.DataField.indexOf(".") == -1) { this.RenderDataTrackLink(); }
            //是否多选
            this.IsMultiple = this.Options.IsMultiple || this.LogicType == $.MvcSheetUI.LogicType.MultiParticipant;

            //不可用
            if (!this.Editable) {
                $(this.Element).prop("readonly", true);
                $(this.Element).addClass(this.Css.Readonly);
                $(this.Element).css("padding-top", "6px");
            } else {
                this.ClearChoices();
                this.IsLoaded = false;
                this.SelectedValue = "";
                this.__QueryOptions = "";

                //渲染界面
                this.HtmlRender();
                //绑定事件
                this.BindEnvens();
            }

            //初始化默认值
            this.InitValue();
        },

        //初始化值
        InitValue: function () {
            // 设置默认值
            if (this.Originate && !this.V && this.DefaultValue) {
                this.V = this.DefaultValue;
                if (this.V.constructor == String) {
                    if (this.V.toLowerCase() == "originator" || this.V.toLowerCase() == "{originator}") {
                        this.V = this.SheetInfo.Originator;
                    } else if (this.V.toLowerCase() == "{originator.ou}" || this.V.toLowerCase() == "originator.ou") {
                        this.V = this.SheetInfo.OriginatorOU;
                    }
                }
            }
            this.SetValue(this.V);
            //if (this.IsMobile) {
            //    if (this.Editable) {
            //        this.Mask.html(this.GetText());
            //    } else {
            //        $(this.Elment).html(this.GetText());
            //    }
            //}
        },
        //设置组织机构的根目录，传组织编码
        SetRootUnit: function (unitId) {
            // 设置顶点 unit 
            // 重新加载树
            this.RootUnitID = RootUnitID;
            this.TreeManager.clear();
            this.TreeManager.loadData(null, this.SheetUserHandler + "?RootUnitID=" + this.RootUnitID);
        },
        //设置值
        SetValue: function (Obj) {
            if (Obj == undefined || Obj == null || Obj == "") {
                if (this.IsMobile) {
                    if (this.Editable) {
                        if (this.PlaceHolder) {
                            this.Mask.text(this.PlaceHolder);
                        } else {
                            this.Mask.text(SheetLanguages.Current.PleaseSelect);
                        }       
                        this.Mask.css({ 'color': '#797f89' });
                    } else {
                        this.Mask.html(this.PlaceHolder);
                    }
                    return;
                } else {
                    return;
                }
            }
            if (Obj.constructor == Object) {
                this.AddChoice({ ObjectID: Obj.Code, Name: Obj.Name, type: Obj.ContentType });
            } else if (Obj.constructor == Array) {
                for (var i = 0; i < Obj.length; i++) {
                    if (Obj[i].constructor == Object) {
                        this.AddChoice({ ObjectID: Obj[i].Code, Name: Obj[i].Name, type: Obj[i].ContentType });
                    } else if (Obj[i].constructor == String) {
                        this.AddUserID(Obj[i]);
                        if (!this.IsMultiple) break;
                    }
                }
            } else if (Obj.constructor == String) {
                this.AddUserID(Obj);
            }

            if (this.IsMobile) {
                if (this.Editable) {
                    this.Mask.html(this.GetText());
                    this.Mask.css({ 'color': '#2c3038' });
                } else {
                    //$(this.Element).html(this.GetText());
                    var txt = this.GetText();
                    var mask = $("<label>").html(this.GetText());
                    $($(this.Element).html("")).append(mask);
                }
            }
        },
        //用户ID
        GetValue: function () {
            var users;
            for (var ObjectID in this.Choices) {
                if (this.IsMultiple) {
                    if (users == undefined) users = new Array();
                    users.push(ObjectID);
                } else {
                    users = ObjectID;
                }
            }
            return users == undefined ? "" : users;
        },
        //转化为对象
        GetChoices: function () {
            var choices;
            for (var ObjectID in this.Choices) {
                if (this.IsMultiple) {
                    if (choices == undefined) choices = new Array();
                    choices.push(this.Choices[ObjectID]);
                } else {
                    choices = this.Choices[ObjectID];
                }
            }
            return choices == undefined ? [] : choices;
        },
        //获取显示
        GetText: function () {
            var userNames;
            for (var ObjectID in this.Choices) {
                if (this.IsMultiple) {
                    if (userNames == undefined) userNames = new Array();
                    userNames.push(this.Choices[ObjectID].Name);
                } else {
                    userNames = this.Choices[ObjectID].Name;
                }
            }
            return userNames == undefined ? "" : userNames.toString();
        },

        //保存数据
        SaveDataField: function () {
            var result = {};
            if (!this.Visiable || !this.Editable) return result;
            result[this.DataField] = this.DataItem;
            if (!result[this.DataField]) {
                if (this.DataField.indexOf(".") == -1) { alert(SheetLanguages.Current.DataItemNotExists + ":" + this.DataField); }
                return {};
            }

            var users = this.GetValue();
            // if (result[this.DataField].V != users)
            {
                result[this.DataField].V = users;
                return result;
            }
            return {};
        },

        //渲染样式
        HtmlRender: function () {
            this.ID = $(this.Element).attr("ID") || $.MvcSheetUI.NewGuid();
            $(this.Element).attr("ID", this.ID);
            $(this.Element).css("z-index", "inherit");

            //设置页面元素的样式
            $(this.Element).addClass("select2-container select2-container-multi ").attr("data-sheetusercontrol", true);
            $(this.Element).css("min-width", "150px");

            //设置输入框
            if (this.ChoicesElement == null) {
                this.ChoicesElement = $("<ul>").addClass("select2-choices").css("overflow", "hidden");
                this.SearchElement = $("<li>").addClass("select2-search-field");
                this.SearchTxtElement = $("<input>").addClass("form-control").addClass("no-padding");
                this.SearchTxtElement.width("1px");

                //添加输入框
                this.SearchElement.append(this.SearchTxtElement);
                this.ChoicesElement.append(this.SearchElement);
                $(this.Element).append(this.ChoicesElement);
            }

            this.SetSearchTxtElementWidth.apply(this);
            //不可用
            if (!this.Editable) {
                $(this.SearchTxtElement).prop("readonly", true);
                $(this.SearchElement).width("100%");
                $(this.SearchElement).addClass(this.Css.Readonly);
                this.SearchTxtElement.closest("ul").css("border", "0px");
                this.SearchTxtElement.width("100%");
                return;
            }
            if (this.SelectorPanel == null) {
                //组织机构选择Panel
                this.SelectorPanel = $("<div>").addClass("bordered").addClass("SelectorPanel")
                    .css("position", "absolute")
                    .css("z-index", "888")
                    // .css("overflow", "auto")
                    .width("100%")
                    .css("min-width", "500px")
                    .css("height", "300px")
                    .css("display", "none")
                    .css("background-color", "#FFFFFF").css("left", "0").attr("data-SheetUserPanel", "SelectorPanel");

                this.OrgTreePanel = $("<div>").css("max-width", "60%").css("width", "60%").addClass("bordered").css("float", "left").height("100%").css("overflow", "scroll");
                this.SelectorPanel.append(this.OrgTreePanel);

                // 组织机构选择列表
                this.OrgListPanel = $("<div>").height("100%").css("overflow", "auto");
                this.SelectorPanel.append(this.OrgListPanel);
                // 添加组织机构
                $(this.Element).append(this.SelectorPanel);
            }

            if (!this.Recursive ||
                this.RoleName ||
                this.OrgPostCode ||
                this.UserCodes) { // 只显示下拉框，不显示左侧菜单
                this.OrgTreePanel.hide();
                this.SelectorPanel.css("min-width", "0px");
            } else {
                this.OrgTreePanel.show();
                this.SelectorPanel.css("min-width", "500px");
            }
        },
        //绑定事件
        BindEnvens: function () {
            if (!this.Editable) return; //不可用

            //点击到当前元素，设置input焦点
            $(this.ChoicesElement).unbind("click.SheetUser").bind("click.SheetUser", this, function (e) {
                e.data.SearchTxtElement.focus();
            });

            //得到焦点显示
            $(this.SearchTxtElement).unbind("focusin.SearchTxtElement").bind("focusin.SearchTxtElement", this, function (e) {
                e.data.FocusInput.apply(e.data);
            });
            if (!this.Recursive) { //不递归的时候，直接显示内容
                var SheetUserManager = this;
                //读取控件上的属性
                $.ajax({
                    type: "GET",
                    url: this.SheetUserHandler + "?Recursive=false&" + this.GetLoadTreeOption(),
                    dataType: "json",
                    //async: false,//同步执行
                    success: function (data) {
                        for (var i = 0; i < data.length; ++i) {
                            SheetUserManager.AddListItem.apply(SheetUserManager, [data[i]]);
                        }
                    }
                });
                if (this.IsMobile) {
                    //this.FocusInput();
                } else {
                    $(document).unbind("click." + this.ID).bind("click." + this.ID, this, function (e) {
                        if ($(e.target).closest("div[id='" + e.data.ID + "']").length == 0) {
                            e.data.FocusOutput.apply(e.data);
                        }
                    });
                }
                $(this.SearchTxtElement).prop("readonly", "readonly");
                return;
            }

            if (this.IsMobile) {
                // 输入控件
                $(this.SearchTxtElement).unbind("keydown.SearchTxtElement").bind("keydown.SearchTxtElement", this, function (e) {
                    if (e.which == 13) {
                        e.preventDefault();
                        e.stopPropagation();
                    }
                });

                // 移动端仅在Enter时执行搜索
                $(this.SearchTxtElement).unbind("keyup.SearchTxtElement").bind("keyup.SearchTxtElement", this, function (e) {
                    if (e.which == 13) {
                        e.data.SetSearchTxtElementWidth.apply(e.data);
                        e.data.FocusInput.apply(e.data);
                        e.data.SearchOrg.apply(e.data, [e.data]);
                    }
                });
            } else {
                // 输入控件
                $(this.SearchTxtElement).unbind("keyup.SearchTxtElement").bind("keyup.SearchTxtElement", this, function (e) {
                    e.data.SetSearchTxtElementWidth.apply(e.data);
                    e.data.FocusInput.apply(e.data);
                    e.data.KeyTime = new Date();
                    // setTimeout("e.data.SearchOrg.apply(e.data, [e.data])", 500);
                    var that = e.data;
                    setTimeout(function () {
                        that.SearchOrg.apply(that, [that]);
                    }, 500)
                });
                $(this.SearchTxtElement).unbind("keydown.SearchTxtElement").bind("keydown.SearchTxtElement", this, function (e) {
                    if (e.keyCode == 8 && $(this).val() == "") {
                        e.data.RemoveChoice.apply(e.data, [$(this).parent().prev().attr("data-code")]);
                    }
                });
            }
            $(document).unbind("click." + this.ID).bind("click." + this.ID, this, function (e) {
                if ($(e.target).closest("div[id='" + e.data.ID + "']").length == 0) {
                    e.data.FocusOutput.apply(e.data);
                }
            });
            if (this.IsMobile) {
                $(this.OrgListBtn).unbind("click.OrgListBtn").bind("click.OrgListBtn", this, function (e) {
                    var targetID = $(this).data("targetID");
                    e.data.AddMobilePanel.apply(e.data, [targetID, ""]);
                });
            }
        },
        //移动端:添加panel
        AddMobilePanel: function (id, parentID) {
            this.Level++;
            var that = this;
            var divObj = $("#" + id);
            if (divObj.length == 0) {
                //新pannel
                divObj = $("<div>").attr('id', id).addClass('panel').addClass('no-scroll').hide();
                if (parentID != "") {
                    var parentObj = $("li[objectID='" + parentID + "']>label");
                    if (parentObj.length == 0) parentObj = $("li[objectID='" + parentID + "']");
                    divObj.attr("data-title", parentObj.text());
                } else {
                    divObj.attr("data-title", $.ui.prevHeader.find("#pageTitle").text());
                }
                divObj.attr("data-footer", this.FooterID);
                divObj.data("parentid", parentID);
                var loadUrl = that.SheetUserHandler + "?IsMobile=true&" + that.GetLoadTreeOption();
                if (parentID) {
                    loadUrl = that.SheetUserHandler + "?IsMobile=true&ParentID=" + parentID + "&" + that.GetLoadTreeOption();
                }

                $.ajax({
                    type: "GET",
                    url: loadUrl,
                    dataType: "json",
                    context: that,
                    async: false, //同步执行
                    success: function (data) {
                        var ul = $("<ul>").addClass('orglist').addClass('list');
                        that.AddMobileOptions(data, ul);

                        var wrapper = $("<div class='scroll-wrapper'>");
                        wrapper.append(ul);
                        divObj.append(wrapper);

                        $('#content').append(divObj);

                        that.SetMobilePanelRefreshOnload(id);
                    }
                });

                //添加
                //$.ui.addContentDiv(id);
            }

            //显示
            $.ui.loadContent(id);
            //检查是否选中
            this.MobileFindCheckbox(id);
        },

        MobilePreBack: function () {
            var id = "#" + $.ui.activeDiv.id;
            if (this.Level > 0) {
                this.Level--;
            }
            this.MobileFindCheckbox(id);
        },

        //设置页面加载时自动刷新滚动条
        SetMobilePanelRefreshOnload: function (panelId) {
            var that = this;
            //进入页面时自动刷新滚动条
            window.PanelLoadActions = window.PanelLoadActions || {};
            var that = this;
            var fnName = 'F' + this.EditPanel.attr('id').replace(/\-/g, '');

            $('#' + panelId).attr('data-load', 'window.PanelLoadActions.' + fnName)

            window.PanelLoadActions[fnName] = function () {
                setTimeout(function () {
                    that.RefreshMobilePage();
                }, 600);
            }
        },


        AddMobileOptions: function (data, ulList, searchKey) {
            if (data) {
                var that = this;
                if (data instanceof Array) {
                    if (data.length) {
                        $(data).each(function () {
                            that._AddMobileOption(this, ulList, searchKey);
                        })
                    } else {
                        ulList.html('<li class="user-item">没有任何组织</li>');
                    }
                } else {
                    that._AddMobileOption(data, ulList, searchKey);
                }
            } else {
                ulList.html('<li class="user-item">没有任何组织</li>');
            }
        },

        //获取是否允许选择组、OU、用户的标识
        GetSelectableFlag: function () {
            if (typeof (this.__SelectableOption) == 'undefined') {
                this.__SelectableOption = '';

                loadOptions = this.GetLoadTreeOption();
                var o = loadOptions.match(/o=[A-z]*/)
                if (o && o.length) {
                    this.__SelectableOption = o[0].replace('o=', '');
                }
            }
            return this.__SelectableOption;
        },

        //添加可选项
        _AddMobileOption: function (item, ulList, searchKey) {
            var selectableFlag = this.GetSelectableFlag();
            var li = $("<li>").addClass('user-item');
            if (selectableFlag.indexOf(item.ExtendObject.UnitType) > -1) {
                var checkboxid = $.uuid();
                var checkbox = $("<input type='checkbox'  id='" + checkboxid + "' data-objectid='" + item.ObjectID + "'/>");
                checkbox.attr("checked", this.Choices && this.Choices[item.ObjectID] != undefined);
                li.append(checkbox);

                var displayText = item.Text;
                if (searchKey) {
                    displayText = displayText.replace(searchKey, "<span class='bg-info'>" + searchKey + "</span>");
                    if (displayText.indexOf("[" + item.Code + "]") == -1) {
                        displayText += "[" + item.Code.replace(searchKey, "<span class='bg-info'>" + searchKey + "</span>") + "]";
                    }
                }

                li.append($("<label type='checkbox' label-for='" + checkboxid + "'>" + displayText + "</label>").css("float", "none").css("left", "25px"));
            } else {
                li.append(item.Text);
            }
            li.attr("objectID", item.ObjectID);
            var targetId = $.uuid();
            li.attr("targetID", targetId);

            if (!item.IsLeaf) {
                var linkelemnt = $("<a data-ignore=true>" + $(li).html() + "</a>");
                $(li).html("").append(linkelemnt);

                li.append($('<div>').addClass('org-expand').css({
                    width: '20%',
                    height: '100%',
                    'z-index': 2,
                    position: 'absolute',
                    right: 0,
                    top: 0
                }));
            }
            ulList.append(li);

            var node = {
                ObjectID: item.ObjectID,
                Name: item.Text
            }
            $(li).unbind("click.OrgListBtn").bind("click.OrgListBtn", [this, node], function (e) {
                var t = e.data[0];
                var n = e.data[1];
                if ($(e.target).is('.org-expand') || $(this).find('input[type=checkbox]').length == 0) {
                    var parentID = $(this).attr("objectID");
                    var targetID = $(this).attr("targetID");
                    $("#defaultHeader>.backButton").data("pannelid", targetId);
                    t.AddMobilePanel.apply(t, [targetID, parentID]);
                } else {
                    var chk = $(this).find("input[type=checkbox]")
                    chk.prop('checked', !chk.prop('checked'));

                    t.UnitCheckboxClick.apply($(this).find("input[type=checkbox]").get(0), e.data);
                }
            });
        },

        //检查是否选中
        MobileFindCheckbox: function (id) {
            if (id == undefined && $.ui.history) {
                id = $.ui.history[$.ui.history.length - 1].target
            }

            if (id.indexOf("#") < 0) {
                id = "#" + id;
            }

            var that = this;
            $(id).find("input:checkbox").each(function () {
                $(this).prop("checked", that.Choices[$(this).attr("data-objectid")] != undefined);
            });
        },

        //设置输入框的宽度
        SetSearchTxtElementWidth: function () {
            if (this.IsMobile) {
                return;
            }
            var w = "1px";
            var length = this.SearchTxtElement.val().length;
            if (length > 0) {
                w = length * 15 + "px";
                this.SearchTxtElement.removeAttr("PlaceHolder", this.PlaceHolder);
            } else if ($.isEmptyObject(this.Choices)) {
                w = "1px";
                this.SearchTxtElement.attr("PlaceHolder", this.PlaceHolder);
            } else {
                this.SearchTxtElement.removeAttr("PlaceHolder", this.PlaceHolder);
            }
            $(this.SearchTxtElement).width(w);

            if (this.IsMobile) {
                //$(this.SelectorPanel).css("top", ($(this.Element).height()+20)+"px");
            }
        },

        //获取焦点焦点
        FocusInput: function () {
            if (this.IsMobile) {
                return;
            }
            if (this.SelectorPanel.is(":visible")) return;

            $("div[data-SheetUserPanel='SelectorPanel']").hide();
            this.SelectorPanel.show();
            if (!this.IsMobile && !this.IsLoaded) {
                //加载组织机构树
                this.LoadOrgTree();
            }

            if (!this.IsMobile && this.OrgListPanel.find("input").length == 0) {
                this.TreeManager.selectNode(this.TreeManager.data[0]);
            }
        },

        //失去焦点
        FocusOutput: function () {
            if (this.IsMobile) {
                return;
            }

            if (this.SelectorPanel.is(":hidden")) return;
            if ($(this.SearchTxtElement).val().length > 0) {
                this.OrgListPanel.html("");
                $(this.SearchTxtElement).val("");
            }
            this.SelectorPanel.hide();
        },

        //处理映射关系
        MappingControlsHandler: function (Object) {
            if (!this.MappingControls) return;

            var Propertys = "";
            var MapJson = {};
            var mapping = this.MappingControls.split(',');
            for (var i = 0; i < mapping.length; i++) {
                var map = mapping[i].split(':');
                MapJson[map[0]] = map[1];
                Propertys += map[1] + ";";
            }

            var that = this;
            var param = { Command: "GetUserProperty", Param: JSON.stringify([Object.ObjectID, Propertys]) };
            $.MvcSheet.GetSheet(param, function (data) {
                for (var p in data) {
                    for (var key in MapJson) {
                        if (MapJson[key] == p) {
                            //var e = $.MvcSheetUI.GetElement(key, that.RowNum);
                            var e = $.MvcSheetUI.GetElement(key, that.GetRowNumber());
                            if (e != null && e.data($.MvcSheetUI.SheetIDKey)) {
                                e.SheetUIManager().SetValue(data[p]);
                            }
                        }
                    }
                }
            });
        },

        //添加选择:{ObjectID:"",Name:""}
        AddChoice: function (Object) {
            if (!Object) return;
            if (!Object.ObjectID) return;
            if (this.Choices[Object.ObjectID]) return;
            if (!this.IsMultiple) { // 清除其他所有选项
                this.ClearChoices();
            }
            this.Choices[Object.ObjectID] = Object;

            //映射关系
            if (this.MappingControls) {
                this.MappingControlsHandler(Object);
            }

            //只读
            if (!this.Editable) {
                $(this.Element).html(this.GetText());
                return;
            }

            var choiceID = $.MvcSheetUI.NewGuid();
            Object.ChoiceID = choiceID;
            var choice = $("<li class='select2-search-choice'></li>")

            var NameDiv = $("<div>" + Object.Name + "</div>");
            choice.css("cursor", "pointer").css("margin-top", "2px").css('background-color', '#b0b0b0');
            choice.attr("id", choiceID).attr("data-code", Object.ObjectID).append(NameDiv);

            if (this.IsMobile) {
                choice.css("margin-top", "10px")
                // this.ChoicesElement.append(choice);
            } else {
                this.SearchElement.before(choice);
                this.SetSearchTxtElementWidth.apply(this);
                // this.ChoicesElement.append(choice);
                choice.insertBefore(this.ChoicesElement.find("li:last"));
            }

            //关闭按钮
            var colseChoice = $("<a href='javascript:void(0)' class='select2-search-choice-close'></a>");
            choice.append(colseChoice);
            choice.unbind("click.choice").bind("click.choice", this, function (e) {
                e.data.RemoveChoice.apply(e.data, [$(this).attr("data-code")]);
            });
            //校验
            this.Validate();

            if (this.IsMobile) {
                this.OnMobileChange();
            }

            if (this.OnChange) {
                this.RunScript(this, this.OnChange, [this]);
            }
        },

        OnMobileChange: function () {
            if (this.IsMobile) {
                var that = this;
                setTimeout(function () {
                    that.RefreshMobilePage();
                }, 100)
            }
        },

        RefreshMobilePage: function () {
            //如果当前在选择的主界面里，重新计算高度
            // if ($.ui.activeDiv.id == this.EditPanelID) {
            if (this.EditPanelID) {
                //选中项容器高度自增减
                this.ChoicesPanel.height($(this.ChoicesElement).outerHeight());

                if (this.SelectorPanel) {
                    //搜索框填充页面高度
                    this.SelectorPanel.outerHeight($('#afui').height() - $('header:visible').outerHeight() - $('#footer:visible').outerHeight() - this.ChoicesPanel.outerHeight() - this.SearchElement.parent().outerHeight())
                }
            }

            var that = this;

        },

        _GetScroller: function (wrapperSelector) {
            this.IScrollers = this.IScrollers || {};

            var wrapper = $(wrapperSelector).first();
            var scrollerId = wrapper.data("scroller-id");
            if (!scrollerId) {
                scrollerId = $.uuid();
                wrapper.data("scroller-id", scrollerId);
                this.IScrollers[scrollerId] = new IScroll(wrapper.get(0));
            }
            return this.IScrollers[scrollerId];;
        },

        //Error:这里有时间，可以实现批量的效果
        //添加:UserID/UserCode
        AddUserID: function (UserID) {
            var that = this;
            var param = { UserID: UserID, Propertystr: "Name;ObjectID" };
            $.ajax({
                type: "GET",
                url: this.SheetGetUserProperty,
                data: param,
                dataType: "json",
                async: false, //同步执行
                success: function (data) {
                    if (data) {
                        that.AddChoice({ ObjectID: data["ObjectID"], Name: data["Name"] });
                    }
                }
            });
        },

        //清除所有的选择
        ClearChoices: function () {
            for (var ObjectID in this.Choices) {
                this.RemoveChoice(ObjectID);
            }
            this.OnMobileChange();
        },

        //移除选择
        RemoveChoice: function (ObjectID) {
            if (this.Choices[ObjectID]) {
                if (!this.IsMobile) {
                    this.OrgListPanel.find("input[ObjectID='" + ObjectID + "']").attr("checked", false);
                }
                $("#" + this.Choices[ObjectID].ChoiceID).remove();
                delete this.Choices[ObjectID];
            }
            this.SetSearchTxtElementWidth.apply(this);
            this.Validate();

            this.OnMobileChange();

            if (this.OnChange) {
                this.RunScript(this, this.OnChange, [this]);
            }
        },

        //加载组织机构树
        LoadOrgTree: function () {
            //加载样式和脚本
            if (!this.OrgTreePanel) return;
            var that = this;
            var treeUl = $("<ul>").css("min-width", "600px");
            this.OrgTreePanel.append(treeUl);

            //加载LigerUI
            if (!treeUl.ligerTree) {
                $("body:first").append("<link rel='stylesheet' type='text/css' href='" + $.MvcSheetUI.PortalRoot + "/WFRes/_Content/themes/ligerUI/Aqua/css/ligerui-tree.css' />");
                $.ajax({
                    url: $.MvcSheetUI.PortalRoot + "/WFRes/_Scripts/ligerUI/ligerui.all.js",
                    type: "GET",
                    dataType: "script",
                    async: false, //同步请求
                    global: false
                });
            }

            this.IsLoaded = false;
            var paramOptions = this.GetLoadTreeOption();
            this.TreeManager = $(treeUl).ligerTree({
                checkbox: false,
                idFieldName: 'Code',
                textFieldName: 'Text',
                iconFieldName: "Icon",
                btnClickToToggleOnly: true,
                isExpand: 2,
                isLeaf: function (data) {
                    return data.IsLeaf;
                },
                delay: function (e) {
                    var node = e.data;
                    if (node == null) return false;
                    if (node.IsLeaf == null) return false;
                    if (!node.IsLeaf && node.children == null) {
                        return {
                            url: that.SheetUserHandler + "?ParentID=" + node.Code + "&LoadTree=true&Recursive=false&" + paramOptions,
                        }
                    }
                    return false;
                },
                url: this.SheetUserHandler + "?LoadTree=true&Recursive=" + this.Recursive + "&" + paramOptions,
                onSelect: this.TreeNodeClick,
                onCancelselect: this.TreeNodeClick,
                SheetUserManager: this,
                onSuccess: function () {
                    if (!this.options.SheetUserManager.IsLoaded) {
                        if (this.data.length > 0) {
                            this.options.SheetUserManager.TreeNodeClick.apply(this, [{ data: this.data[0] }]);
                        }
                        this.options.SheetUserManager.IsLoaded = true;
                    }
                }
            });
        },
        //获取加载组织机构的参数
        GetLoadTreeOption: function () {
            if (!this.__QueryOptions) {
                var querystr = "o=";
                var querystr = "o=";
                if (this.SegmentVisible) {
                    querystr += "S";
                }
                if (this.OrgUnitVisible) {
                    querystr += "O";
                }
                if (this.GroupVisible) {
                    querystr += "G";
                }
                if (this.PostVisible) {
                    querystr += "P";
                }
                if (this.UserVisible) {
                    querystr += "U";
                }
                if (this.VisibleUnits) {
                    querystr += "&VisibleUnits=" + this.VisibleUnits;
                }
                if (this.RootUnitID) {
                    querystr += "&RootUnitID=" + this.RootUnitID;
                }
                //if (this.RoleName) {
                //    querystr += "&RoleName=" + encodeURI(this.RoleName);
                //}
                if (this.OrgPostCode) {
                    querystr += "&OrgPostCode=" + encodeURI(this.OrgPostCode);
                }
                if (this.UserCodes) {
                    querystr += "&UserCodes=" + encodeURI(this.UserCodes);
                }

                this.__QueryOptions = querystr;
            }
            return this.__QueryOptions;
        },
        //点击节点
        TreeNodeClick: function (e) {
            if (e == null) return;
            if (e.data == null) return;
            var node = e.data;

            var SheetUserManager = this.options.SheetUserManager;
            if (SheetUserManager.SelectedValue == node.ObjectID && !SheetUserManager.SearchMode) return;
            SheetUserManager.SelectedValue = node.ObjectID;
            SheetUserManager.SearchMode = false;
            // 读取控件上的属性
            var querystr = SheetUserManager.GetLoadTreeOption();
            querystr = querystr.replace("&VisibleUnits=", "&V=");
            SheetUserManager.OrgListPanel.html("");

            if (e.data.treedataindex == 0 &&
                SheetUserManager.OrgUnitVisible &&
                SheetUserManager.RootSelectable) {
                SheetUserManager.AddListItem.apply(SheetUserManager, [e.data]);
            }

            $.ajax({
                type: "GET",
                url: SheetUserManager.SheetUserHandler + "?ParentID=" + node.ObjectID + "&" + querystr,
                dataType: "json",
                //async: false,//同步执行
                success: function (data) {
                    for (var i = 0; i < data.length; ++i) {
                        SheetUserManager.AddListItem.apply(SheetUserManager, [data[i]]);
                    }
                }
            });
        },
        //搜索用户
        SearchOrg: function (SheetUserManager) {
            var searchkey = $(SheetUserManager.SearchTxtElement).val().trim();
            var currentTime = new Date();
            if ((currentTime - this.KeyTime) < 499) return;
            if (!SheetUserManager.SearchKey && !searchkey) {
                return;
            }
            if (!searchkey) {
                if (!this.IsMobile) {
                    SheetUserManager.SearchKey = searchkey;
                    SheetUserManager.OrgListPanel.html("");
                    // SheetUserManager.TreeManager.selectNode(0);
                    SheetUserManager.TreeManager.selectNode(SheetUserManager.TreeManager.nodes[0]);
                } else {
                    SheetUserManager.OrgListPanel.html("");
                }
                return;
            }
            SheetUserManager.SearchMode = true;
            SheetUserManager.SearchKey = searchkey;
            SheetUserManager.OrgListPanel.html("");

            for (var k in SheetUserManager.SearchResults) {
                if (SheetUserManager.SearchResults[k].SearchKey == searchkey &&
                    SheetUserManager.SearchResults[k].ParentID == SheetUserManager.SelectedValue) {
                    if (SheetUserManager.SearchResults[k].Data && SheetUserManager.SearchResults[k].Data.length) {
                        for (var i = 0; i < SheetUserManager.SearchResults[k].Data.length; ++i) {
                            SheetUserManager.AddListItem.apply(SheetUserManager, [SheetUserManager.SearchResults[k].Data[i], searchkey]);
                        }
                    } else {
                        SheetUserManager.OrgListPanel.html('<li class="user-item">没有任何组织</li>');
                    }
                    return;
                }
            }
            $.ajax({
                type: "GET",
                url: SheetUserManager.SheetUserHandler + "?IsMobile=" + (!!this.IsMobile) +
                    "&SearchKey=" + encodeURI(searchkey) +
                    "&ParentID=" + (SheetUserManager.SelectedValue || SheetUserManager.RootUnit || "") +
                    "&" + SheetUserManager.GetLoadTreeOption(),
                dataType: "json",
                //async: false,//同步执行
                success: function (data) {
                    if (SheetUserManager.IsMobile) {
                        SheetUserManager.AddMobileOptions(data, SheetUserManager.OrgListPanel, searchkey);

                        setTimeout(function () {
                            SheetUserManager.RefreshMobilePage();
                        }, 550);
                    } else {
                        if (data != null && data.length > 0) {
                            for (var i = 0; i < data.length; ++i) {
                                SheetUserManager.AddListItem.apply(SheetUserManager, [data[i], searchkey]);
                            }
                        } else {
                            SheetUserManager.OrgListPanel.html("<li class='user-item'>没搜索到组织</li>");
                        }
                    }
                    SheetUserManager.SearchResults.push({ SearchKey: searchkey, ParentID: SheetUserManager.SelectedValue, Data: data });
                }
            });
        },

        //添加组织机构选择列项
        AddListItem: function (node, searchkey) {
            var displayText = node.Text;
            if (searchkey) {
                displayText = displayText.replace(searchkey, "<span class='bg-info'>" + searchkey + "</span>");
                if (node.Code != "" && node.Text.indexOf("[" + node.Code + "]") == -1) {
                    displayText += "[" + node.Code.replace(searchkey, "<span class='bg-info'>" + searchkey + "</span>") + "]";
                }
            }
            if (this.IsMobile) {
                var item = $("<div></div>");
                item.css("border-bottom", "1px solid #ccc");
                item.height("50px");
                item.css("clear", "both");
                var checkid = $.MvcSheetUI.NewGuid();
                var checkbox = $("<input type='checkbox' ObjectID='" + node.ObjectID + "' id='" + checkid + "'/>");
                item.append(checkbox);
                item.append($("<label type='checkbox' label-for='" + checkid + "'>" + displayText + "</label>").css("float", "none").css("left", "25px"));
                this.OrgListPanel.append(item);

                var thatSheetUser = this;
                item.bind("touchstart", function () {
                    checkbox.click();

                    thatSheetUser.UnitCheckboxClick.apply(checkbox.get(0), [thatSheetUser, { ObjectID: node.ObjectID, Name: node.Text }]);
                });
            } else {
                var item = $("<div>").addClass("task").append("<i class='task-sort-icon fa  " + node.Icon + "'></i>").append("&nbsp;<span class='task-title' style='word-break: break-all;'>" + displayText + "</span>");
                item.css("padding", "5px 10px 5px 10px").css("border-bottom", "1px solid #e4e4e4").css("cursor", "pointer");
                var checkid = $.MvcSheetUI.NewGuid();
                var checkbox = $("<input type='checkbox' ObjectID='" + node.ObjectID + "' id='" + checkid + "'/>").css("height", "auto").css("margin", "0px").show();
                var checkItem = $("<div>").addClass("action-checkbox pull-right").append(checkbox);

                item.click(function (e) {
                    if (e.target.tagName.toLowerCase() != "input") {
                        $(this).find("input").click();
                    }
                });
                this.OrgListPanel.append(item.append(checkItem));
                checkbox.attr("checked", this.Choices[node.ObjectID] != undefined);

                checkbox.click({ that: this, option: { ObjectID: node.ObjectID, Name: node.Text } }, function (e) {
                    e.data.that.UnitCheckboxClick.apply(this, [e.data.that, e.data.option]);
                });
            }
        },

        //单元选人点击
        UnitCheckboxClick: function (that, node) {
            if (this.checked) {
                if (!that.IsMultiple) {
                    that.ClearChoices.apply(that);
                    that.OrgListPanel.find("input").attr("checked", false);
                    this.checked = true;
                    if (that.IsMobile && that.Level > 0) {
                        $.ui.goBack(that.Level);
                        that.Level = 0;
                    }
                    that.FocusOutput.apply(that);
                }
                that.AddChoice.apply(that, [node]);
            } else {
                if (that.IsMultiple)
                    that.RemoveChoice.apply(that, [node.ObjectID]);
                else
                    that.ClearChoices.apply(that);
            }

        }
    });
})(jQuery);