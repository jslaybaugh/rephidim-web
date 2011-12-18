﻿ko.bindingHandlers["groupedOptions"] = {
	init: function (element, valueAccessor, allBindingsAccessor)
	{
		if (element.tagName != "SELECT")
			throw new Error("options binding applies only to SELECT elements");

		var previousSelectedValues = ko.utils.arrayMap(ko.utils.arrayFilter(element.childNodes, function (node)
		{
			return node.tagName && node.tagName == "OPTION" && node.selected;
		}), function (node)
		{
			return ko.selectExtensions.readValue(node) || node.innerText || node.textContent;
		});
		var previousScrollTop = element.scrollTop;

		var value = ko.utils.unwrapObservable(valueAccessor());
		var selectedValue = element.value;

		if (value)
		{
			var allBindings = allBindingsAccessor();
			if (typeof value.length != "number")
				value = [value];
			if (allBindings['optionsCaption'])
			{
				var option = document.createElement("OPTION");
				option.innerHTML = allBindings['optionsCaption'];
				ko.selectExtensions.writeValue(option, undefined);
				element.appendChild(option);
			}

			var optionsGroupNamesValue = allBindings['optionsGroupNames'];

			// Group values into optgroups
			var groupedOptions = [];
			var optionsGroupValue = allBindings['optionsGroup']; // undefined if not given
			for (var i = 0, j = value.length; i < j; i++)
			{
				var optionsGroup = null;
				if (typeof optionsGroupValue == "function")
					optionsGroup = optionsGroupValue(value[i]);
				else if (typeof optionsGroupValue == "string")
					optionsGroup = value[i][optionsGroupValue];
				else
					optionsGroup = "";
				if (typeof groupedOptions[optionsGroup] == "undefined")
					groupedOptions[optionsGroup] = [];
				groupedOptions[optionsGroup].push(value[i]);
			}

			// Create HTML elements
			for (var groupName in groupedOptions)
			{
				var optgroup = null;
				// Add an OPTGROUP for all groups except for ""
				if (groupName != "")
				{
					optgroup = document.createElement("OPTGROUP");
					optgroup.label = groupName;
					element.appendChild(optgroup);
				}

				// Create HTML elements for options within this group
				for (var i = 0, j = groupedOptions[groupName].length; i < j; i++)
				{
					var valueGroup = groupedOptions[groupName];
					var option = document.createElement("OPTION");
					var optionValue = typeof allBindings['optionsValue'] == "string" ? valueGroup[i][allBindings['optionsValue']] : valueGroup[groupName][i];

					// Pick some text to appear in the drop-down list for this data value
					var optionsTextValue = allBindings['optionsText'];
					if (typeof optionsTextValue == "function")
						optionText = optionsTextValue(valueGroup[i]); // Given a function; run it against the data value
					else if (typeof optionsTextValue == "string")
						optionText = valueGroup[i][optionsTextValue]; // Given a string; treat it as a property name on the data value
					else
						optionText = optionValue; // Given no optionsText arg; use the data value itself

					optionValue = ko.utils.unwrapObservable(optionValue);
					optionText = ko.utils.unwrapObservable(optionText);
					ko.selectExtensions.writeValue(option, optionValue);
					//SPECIFIC FTS
					if (optionValue == "subject")
					{
						option.setAttribute("data-bind", "enable : !fts.tools.isThereASubjectWidget()");
						ko.applyBindings(fts.main, option);
					}
					//END of specific FTS
					option.innerHTML = optionText.toString();

					if (optgroup != null)
						optgroup.appendChild(option);
					else
						element.appendChild(option);
				}
			}

			// IE6 doesn't like us to assign selection to OPTION nodes before they're added to the document.
			// That's why we first added them without selection. Now it's time to set the selection.
			var newOptions = element.getElementsByTagName("OPTION");
			var countSelectionsRetained = 0;
			for (var i = 0, j = newOptions.length; i < j; i++)
			{
				if (ko.utils.arrayIndexOf(previousSelectedValues, ko.selectExtensions.readValue(newOptions[i])) >= 0)
				{
					ko.utils.setOptionNodeSelectionState(newOptions[i], true);
					countSelectionsRetained++;
				}
			}

			if (previousScrollTop)
				element.scrollTop = previousScrollTop;
		}
	}
};

ko.bindingHandlers['selectedOptions'] = {
	getSelectedValuesFromSelectNode: function (selectNode)
	{
		var result = [];
		var nodes = selectNode.childNodes;
		for (var i = 0, j = nodes.length; i < j; i++)
		{
			var node = nodes[i];
			if ((node.tagName == "OPTGROUP") && node.childNodes != null)
			{
				var subResult = this.getSelectedValuesFromSelectNode(node);
				for (var k = 0; k < subResult.length; k++)
				{
					result.push(subResult[k]);
				}
			}
			else
			{
				if ((node.tagName == "OPTION") && node.selected)
					result.push(ko.selectExtensions.readValue(node));
			}
		}
		return result;
	},
	setSelectedValuesFromSelectNode: function (selectNode, newValue)
	{
		var nodes = selectNode.childNodes;
		for (var i = 0, j = nodes.length; i < j; i++)
		{
			var node = nodes[i];
			if (node.tagName == "OPTGROUP" && node.childNodes != null)
			{
				ko.bindingHandlers['selectedOptions'].setSelectedValuesFromSelectNode(node, newValue);
			}
			else
			{
				if (node.tagName == "OPTION")
					ko.utils.setOptionNodeSelectionState(node, ko.utils.arrayIndexOf(newValue, ko.selectExtensions.readValue(node)) >= 0);
			}
		}
	},
	'init': function (element, valueAccessor, allBindingsAccessor)
	{
		//log('init', element, valueAccessor, allBindingsAccessor);
		ko.utils.registerEventHandler(element, "change", function ()
		{
			var value = valueAccessor();
			if (ko.isWriteableObservable(value))
				value(ko.bindingHandlers['selectedOptions'].getSelectedValuesFromSelectNode(this));
			else
			{
				var allBindings = allBindingsAccessor();
				if (allBindings['_ko_property_writers'] && allBindings['_ko_property_writers']['value'])
					allBindings['_ko_property_writers']['value'](ko.bindingHandlers['selectedOptions'].getSelectedValuesFromSelectNode(this));
			}
		});
	},
	'update': function (element, valueAccessor)
	{
		//log('update', element, valueAccessor);
		if (element.tagName != "SELECT")
			throw new Error("values binding applies only to SELECT elements");

		var newValue = ko.utils.unwrapObservable(valueAccessor());
		if (newValue && typeof newValue.length == "number")
		{
			ko.bindingHandlers['selectedOptions'].setSelectedValuesFromSelectNode(element, newValue);
		}
	}
};


ko.bindingHandlers['tinymce'] = {
	init: function (element, valueAccessor, allBindingsAccessor, context)
	{
		var options = allBindingsAccessor().tinymceOptions || {};
		var modelValue = valueAccessor();
		var value = ko.utils.unwrapObservable(valueAccessor());
		var el = $(element);

		options.setup = function (ed)
		{
			ed.onKeyUp.add(function (ed, evt)
			{ //handle edits made in the editor. Updates after an undo point is reached.
				if (ko.isWriteableObservable(modelValue))
				{
					modelValue(ed.getContent({ format: 'raw' }));
				}
			});

			ed.onInit.add(function (ed, evt)
			{ // Make sure observable is updated when leaving editor.
				var dom = ed.dom;
				var doc = ed.getDoc();
				tinymce.dom.Event.add(doc, 'change', function (e)
				{
					if (ko.isWriteableObservable(modelValue))
					{
						modelValue(ed.getContent({ format: 'raw' }));
					}
				});
			});

		};

		//handle destroying an editor (based on what jQuery plugin does)
		ko.utils.domNodeDisposal.addDisposeCallback(element, function ()
		{
			$(element).parent().find("span.mceEditor,div.mceEditor").each(function (i, node)
			{
				var ed = tinyMCE.get(node.id.replace(/_parent$/, ""));
				if (ed)
				{
					ed.remove();
				}
			});
		});

		//$(element).tinymce(options);
		setTimeout(function () { $(element).tinymce(options); }, 0);
		el.val(value);

	},
	update: function (element, valueAccessor, allBindingsAccessor, context)
	{
		var el = $(element);
		var value = ko.utils.unwrapObservable(valueAccessor());
		var id = el.attr('id');

		//handle programmatic updates to the observable
		// also makes sure it doesn't update it if it's the same.
		// otherwise, it will reload the instance, causing the cursor to jump.
		if (id !== undefined && id !== '')
		{
			tinyMCE.triggerSave();

			var box = tinyMCE.getInstanceById(id);
			if (box != null)
			{
				$("#" + box.id).valid();
				var content = box.getContent({ format: 'raw' });
				if (content !== value)
				{
					el.val(value);
				}
			}
		}
	}
};


ko.bindingHandlers["templateWithOptions"] = {
	init: ko.bindingHandlers.template.init,
	update: function (element, valueAccessor, allBindingsAccessor, viewModel, context)
	{
		var options = ko.utils.unwrapObservable(valueAccessor());
		//if options were passed attach them to $data
		if (options.templateOptions)
		{
			context.$data.$item = ko.utils.unwrapObservable(options.templateOptions);
		}
		//call actual template binding
		ko.bindingHandlers.template.update(element, valueAccessor, allBindingsAccessor, viewModel, context);
		//clean up
		delete context.$data.$item;
	}
}