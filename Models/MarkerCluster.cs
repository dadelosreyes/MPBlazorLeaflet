using BlazorLeaflet.Exceptions;
using BlazorLeaflet.Models.Events;
using BlazorLeaflet.Utils;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlazorLeaflet.Models
{
    public class MarkerCluster : InteractiveLayer
    {
        private readonly IJSRuntime _jsRuntime;

        public MarkerCluster(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        }

        public List<Marker> Markers { get; set; }
        /// <summary>
        /// When you mouse over a cluster it shows the bounds of its markers.
        /// </summary>
        public bool ShowCoverageOnHover { get; set; } = true;

        /// <summary>
        /// When you click a cluster we zoom to its bounds.
        /// </summary>
        public bool ZoomToBoundsOnClick { get; set; } = true;
        
        /// <summary>
        /// When you click a cluster at the bottom zoom level we spiderfy it so you can see all of its markers. (Note: the spiderfy occurs at the current zoom level if all items within the cluster are still clustered at the maximum zoom level or at zoom specified by disableClusteringAtZoom option).
        /// </summary>
        public bool SpiderfyOnMaxZoom { get; set; } = true;
        
        /// <summary>
        /// Clusters and markers too far from the viewport are removed from the map for performance.
        /// </summary>
        public bool RemoveOutsideVisibleBounds { get; set; } = true;
        
        /// <summary>
        /// Smoothly split / merge cluster children when zooming and spiderfying.If L.DomUtil.TRANSITION is false, this option has no effect (no animation is possible).
        /// </summary>
        public bool Animate { get; set; } = true;

        /// <summary>
        /// If set to true (and animate option is also true) then adding individual markers to the MarkerClusterGroup after it has been added to the map will add the marker and animate it into the cluster.Defaults to false as this gives better performance when bulk adding markers.addLayers does not support this, only addLayer with individual Markers.
        /// </summary>
        public bool AnimateAddingMarkers { get; set; } = false;

        /// <summary>
        /// If set, at this zoom level and below, markers will not be clustered.This defaults to disabled. See Example. Note: you may be interested in disabling spiderfyOnMaxZoom option when using disableClusteringAtZoom.
        /// </summary>
        public bool? DisableClusteringAtZoom { get; set; }

        /// <summary>
        /// The maximum radius that a cluster will cover from the central marker(in pixels). Default 80. Decreasing will make more, smaller clusters.You can also use a function that accepts the current map zoom and returns the maximum cluster radius in pixels.
        /// </summary>
        public short MaxClusterRadius { get; set; } = 80;

        /// <summary>
        /// Options to pass when creating the L.Polygon(points, options) to show the bounds of a cluster.Defaults to empty, which lets Leaflet use the default Path options.
        /// </summary>
        //public Polyline<Point[][]> PolygonOptions { get; set; }

        /// <summary>
        /// If set to true, overrides the icon for all added markers to make them appear as a 1 size cluster. Note: the markers are not replaced by cluster objects, only their icon is replaced.Hence they still react to normal events, and option disableClusteringAtZoom does not restore their previous icon (see #391).
        /// </summary>
        public bool SingleMarkerMode { get; set; }

        /// <summary>
        /// Allows you to specify PolylineOptions to style spider legs.By default, they are { weight: 1.5, color: '#222', opacity: 0.5 }.
        /// </summary>
        //public Polyline<Point[][]> SpiderLegPolylineOptions { get; set; }

        /// <summary>
        /// Increase from 1 to increase the distance away from the center that spiderfied markers are placed.Use if you are using big marker icons(Default: 1).
        /// </summary>
        public short SpiderfyDistanceMultiplier { get; set; } = 1;

        //IconCreateFunction: Function used to create the cluster icon.See the default implementation or the custom example.
        //spiderfyShapePositions: Function used to override spiderfy default shape positions.
        //clusterPane: Map pane where the cluster icons will be added.Defaults to L.Marker's default (currently 'markerPane').

        /// <summary>
        /// Boolean to split the addLayers processing in to small intervals so that the page does not freeze.
        /// </summary>
        public bool ChunkedLoading { get; set; } = false;

        /// <summary>
        /// Time interval (in ms) during which addLayers works before pausing to let the rest of the page process. In particular, this prevents the page from freezing while adding a lot of markers. Defaults to 200ms.
        /// </summary>
        public short ChunkInterval { get; set; } = 200;

        /// <summary>
        /// Time delay (in ms) between consecutive periods of processing for addLayers. Default to 50ms.
        /// </summary>
        public short ChunkDelay { get; set; } = 50;

        /// <summary>
        /// Callback function that is called at the end of each chunkInterval. Typically used to implement a progress indicator, e.g. code in RealWorld 50k. Defaults to null. Arguments:
        ///Number of processed markers
        ///Total number of markers being added
        ///Elapsed time(in ms)
        /// </summary>
        public short? ChunkProgress { get; set; }

        public async Task RemoveToClusterLayer(Layer layer) 
        {
            await LeafletInterops.RemoveToClusterLayer(_jsRuntime, Id, layer);
        }

        public delegate void MarkerClusterEventHandler(MarkerCluster sender, Event e);

        public event MarkerClusterEventHandler OnMove;

        [JSInvokable]
        public void NotifyMove(Event eventArgs)
        {
            OnMove?.Invoke(this, eventArgs);
        }

    }
}
