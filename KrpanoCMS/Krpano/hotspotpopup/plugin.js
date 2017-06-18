function krpanoplugin () {

	// the krpano and plugin interface objects
	window.krpano;
	window.plugin;
	window.pluginPath;
	window.adminModal;
	window.layers = [];
	window.api = {
		isAdmin: false,
		getHotspots: function () {
			console.log('krpanoAPI.getHotspots is not implemented.');
		},
		saveHotspot: function () {
			console.log('krpanoAPI.saveHotspot is not implemented.');
		},
		deleteHotspot: function () {
			console.log('krpanoAPI.deleteHotspot is not implemented.');
		},
		getLinks: function () {
			console.log('krpanoAPI.getLinks is not implemented.');
		},
		saveLink: function () {
			console.log('krpanoAPI.saveLink is not implemented.');
		},
		deleteLink: function () {
			console.log('krpanoAPI.deleteLink is not implemented.');
		}
	}
	
	// registerplugin - startup point for the plugin (required)
	// - krpanointerface = krpano interface object
	// - pluginpath = the fully qualified plugin name (e.g. "plugin[name]")
	// - pluginobject = the xml plugin object itself
	this.registerplugin = function (krpanointerface, pluginpath, pluginobject) {
		krpano = krpanointerface;
		plugin = pluginobject;

		pluginPath = plugin.url.split('plugin.js')[0];
		krpano.events.onclick = _handlePanoramaClick.bind(this);

		_loadConfiguration();
		_loadStyles();
		_loadAdminUI();
		_loadHotspots();
	};

	// unloadplugin - exit point for the plugin (optionally)
	// - will be called from krpano when the plugin will be removed
	// - everything that was added by the plugin should be removed here
	this.unloadplugin = function () {
		plugin = null;
		krpano = null;
		api = null;
		layers = null;
		adminModal = null;
	};
	
	function _handlePanoramaClick () {
		if (api.isAdmin) {
			_openCreateDialog();
		} else {
			_closeAllPopups();
		}
	}

	function _loadConfiguration () {
	    if (window.krpanoAPI) {
        api.isAdmin = krpanoAPI.admin;
        if (typeof krpanoAPI.getHotspots == 'function') api.getHotspots = krpanoAPI.getHotspots;
        if (typeof krpanoAPI.saveHotspot == 'function') api.saveHotspot = krpanoAPI.saveHotspot;
        if (typeof krpanoAPI.deleteHotspot == 'function') api.deleteHotspot = krpanoAPI.deleteHotspot;
        if (typeof krpanoAPI.getLinks == 'function') api.getLinks = krpanoAPI.getLinks;
        if (typeof krpanoAPI.saveLink == 'function') api.saveLink = krpanoAPI.saveLink;
        if (typeof krpanoAPI.deleteLink == 'function') api.deleteLink = krpanoAPI.deleteLink;
		}
	}
	
	function _loadStyles () {
		var styleTag = document.createElement('link');
		styleTag.setAttribute('rel', 'stylesheet');
		styleTag.setAttribute('href', pluginPath + 'plugin.css');
		plugin.sprite.appendChild(styleTag);
	}
	
	function _loadAdminUI () {
		if (!api.isAdmin) return;
		adminModal = document.createElement('div');
		adminModal.classList.add('admin-modal-wrapper');
		adminModal.addEventListener('click', _onAdminModalClick);
		document.querySelector('#krpanoSWFObject').appendChild(adminModal);
	}
	
	function _onAdminModalClick (e) {
		if (e.target == adminModal) {
			_closeAdminModal();
			return;
		}
		if (e.target.classList.contains('create-hotspot')) {
			_renderHotspotDialog();
			return;
		}
		if (e.target.classList.contains('create-link')) {
			_renderLinkDialog();
			return;
		}
		if (e.target.classList.contains('admin-cancel')) {
			_closeAdminModal();
			return;
		}
		if (e.target.classList.contains('admin-save-hotspot')) {
			_saveHotspot(e);
			return;
		}
		if (e.target.classList.contains('admin-delete-hotspot')) {
			_deleteHotspot(e);
			return;
		}
		if (e.target.classList.contains('admin-save-link')) {
			_saveLink(e);
			return;
		}
		if (e.target.classList.contains('admin-delete-link')) {
			_deleteLink(e);
			return;
		}
	}
	
	function _openCreateDialog () {
		var coords = krpano.screentosphere(krpano.mouse.x, krpano.mouse.y);
		krpano.call('lookto(' + coords.x + ',' + coords.y +')');
		_openAdminModal(coords);
		if (krpano.scene.getArray().length == 0) {
			_renderHotspotDialog();
		} else {
			_renderCreateDialog();
		}
	}
	
	function _openEditDialog (hotspot) {
		_openAdminModal();
		if (hotspot.linkedscene) {
			_renderLinkDialog(hotspot);
		} else {
			_renderHotspotDialog(hotspot);
		}
	}
	
	function _renderCreateDialog () {
		adminModal.innerHTML = ''
			+ '<div class="admin-modal">'
				+ '<h2>What do you want to create:</h2>'
				+ '<button class="create-button create-hotspot">Create a Hotspot</button>'
				+ '<button class="create-button create-link">Create a Link</button>'
			+ '</div>';
	}
	
	function _renderHotspotDialog (hotspot) {
		adminModal.innerHTML = ''
			+ '<div class="admin-modal">'
				+ '<h2>Create a Hotspot:</h2>'
				+ '<textarea class="admin-field" name="' + (hotspot ? hotspot.name : '') + '">'
					+ (hotspot ? hotspot.content : '')
				+ '</textarea>'
				+ '<button class="admin-save-hotspot">Save</button>'
				+ '<button class="admin-cancel">Cancel</button>'
				+ (hotspot ? '<button class="admin-delete-hotspot">Delete</button>' : '')
			+ '</div>';
	}
	
	function _renderLinkDialog (link) {
		adminModal.innerHTML = ''
			+ '<div class="admin-modal">'
				+ '<h2>Create a Link:</h2>'
				+ '<div class="scenes-list" name="' + (link ? link.name : '') + '">'
					+ (krpano.scene.getArray().map(function (scene, index) {
						return ''
							+ '<label for="sceneItem' + index + '" class="scene-item">'
								+ '<img src="' + scene.thumburl + '">'
								+ '<input type="radio" name="' + (link ? link.name : 'scene') + '" '
									+ (link && link.linkedscene == scene.name ? 'checked ' : '')
									+ 'id="sceneItem' + index + '" value="' + scene.name + '"/>'
							+ '</label>';
					}).join(''))
				+ '</div>'
				+ '<button class="admin-save-link">Save</button>'
				+ '<button class="admin-cancel">Cancel</button>'
				+ (link ? '<button class="admin-delete-link">Delete</button>' : '')
			+ '</div>';
	}
	
	function _openAdminModal (coords) {
		adminModal.coords = coords;
		adminModal.classList.add('open');
	}
	
	function _closeAdminModal () {
		adminModal.coords = null;
		adminModal.classList.remove('open');
		adminModal.innerHTML = '';
	}
	
	function _saveHotspot (e) {
		var field = e.target.parentNode.querySelector('.admin-field');
		var content = field.value;
		if (!content) {
			alert('Please enter Hotspot content!');
			return;
		}
		
		var hotspot = krpano.hotspot.getItem(field.name);
		if (hotspot) {
			hotspot.content = content;
		} else {
			hotspot = _createHotspot({
				name: 'click' + Math.round(krpano.timertick),
				Coordinate_X: adminModal.coords.x,
				Coordinate_Y: adminModal.coords.y,
				Content: content
			});
		}
		
    	api.saveHotspot(_hotspotToDTO(hotspot), function (recordId) {
        	hotspot.recordId = hotspot.recordId || recordId;
    	});
		_closeAdminModal();
	}

	function _hotspotToDTO (hotspot) {
		return {
	    	Id: hotspot.recordId,
	 		Coordinate_X: hotspot.ath,
	 		Coordinate_Y: hotspot.atv,
	 		Content: hotspot.content
		};
	}
	
	function _deleteHotspot (e) {
		var field = e.target.parentNode.querySelector('.admin-field');
		var hotspot = krpano.hotspot.getItem(field.name);
		krpano.removehotspot(hotspot.name);
		api.deleteHotspot(hotspot.recordId);
		_closeAdminModal();
	}
	
	function _saveLink (e) {
		var checkbox = document.querySelector('input[type=radio]:checked');
		if (!checkbox) {
			alert('Please select which panorama to link to!');
			return;
		}
		
		var link = krpano.hotspot.getItem(checkbox.name);
		if (link) {
			link.linkedscene = checkbox.value;
		} else {
			link = _createLink({
				name: 'click' + Math.round(krpano.timertick),
				Coordinate_X: adminModal.coords.x,
				Coordinate_Y: adminModal.coords.y,
				Scene: checkbox.value
			});
		}
		
		api.saveLink(_linkToDTO(link), function (recordId) {
			link.recordId = link.recordId || recordId;
		});
		_closeAdminModal();
	}

	function _linkToDTO (link) {
		return {
	    	Id: link.recordId,
	 		Coordinate_X: link.ath,
	 		Coordinate_Y: link.atv,
	 		Scene: link.linkedscene
		};
	}
	
	function _deleteLink (e) {
		var list = e.target.parentNode.querySelector('.scenes-list');
		var link = krpano.hotspot.getItem(list.getAttribute('name'));
		krpano.removehotspot(link.name);
		api.deleteLink(link.recordId);
		_closeAdminModal();
	}

	function _loadHotspots () {
		api.getHotspots(function (results) {
			results.map(function (item, index) {
				item.name = 'hotspot-' + index;
				_createHotspot(item);
			});
		});
		if (krpano.scene.getArray().length) {
			api.getLinks(function (results) {
				results.map(function (item, index) {
					item.name = 'link-' + index;
					_createLink(item);
				});
			});
		}
	}
	
	function _openPopup (hotspot) {
		_closeAllPopups();
		krpano.call('looktohotspot(' + hotspot.name + ')');
		hotspot.zorder = 20;
		
		var layer = krpano.addlayer(hotspot.name);
		layer.type = 'text';
		layer.parent = hotspot.getfullpath();
		layers.push(layer);
		
		var popup = document.createElement('div');
		popup.classList.add('popup');
		popup.innerHTML = hotspot.content;
		layer.sprite.appendChild(popup);
	}
	
	function _closePopup (layer) {
		var hotspot = krpano.get(layer.parent);
		layers.splice(layers.indexOf(layer), 1);
		krpano.removelayer(layer.name);
		hotspot.zorder = 10;
	}
	
	function _closeAllPopups () {
		layers.map(_closePopup);
	}
	
	function _createHotspot (data) {
		var hotspot = krpano.addhotspot(data.name);
		hotspot.recordId = data.Id;
		hotspot.atv = data.Coordinate_Y;
		hotspot.ath = data.Coordinate_X;
		hotspot.content = data.Content;
		hotspot.distorted = true;
		hotspot.zorder = 10;
		hotspot.url = pluginPath + 'pin.png';
		hotspot.onclick = function () {
			if (api.isAdmin) {
				_openEditDialog(hotspot);
			} else {
				_openPopup(hotspot);
			}
		};
		return hotspot;
	}
	
	function _createLink (data) {
		var link = krpano.addhotspot(data.name);
		link.loadstyle('skin_hotspotstyle');
		link.recordId = data.Id;
		link.atv = data.Coordinate_Y;
		link.ath = data.Coordinate_X;
		link.linkedscene = data.Scene;
		link.zorder = 10;
		if (api.isAdmin) {
			link.onclick = _openEditDialog.bind(this, link);
		}
		return link;
	}
}