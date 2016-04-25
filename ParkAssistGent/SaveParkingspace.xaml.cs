﻿using ParkAssistGent.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace ParkAssistGent
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SaveParkingspace : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private Geoposition parkingspace;

        public SaveParkingspace()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            ShowMyLocationOnTheMap();
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        private async void ShowMyLocationOnTheMap()
        {
            Geoposition geoposition = await getLocation();


            mapWithMyLocation.Center = geoposition.Coordinate.Point;
            MapIcon mapIcon = new MapIcon();
            //mapIcon.Image = RandomAccessStreamReference.CreateFromUri(
              //new Uri("ms-appx:///Assets/pin.png"));
            mapIcon.NormalizedAnchorPoint = new Point(0.25, 0.9);
            mapIcon.Location = geoposition.Coordinate.Point;
            mapIcon.Title = "You are here";
            mapWithMyLocation.MapElements.Add(mapIcon);
            mapWithMyLocation.ZoomLevel = 15;

            //Windows.UI.Xaml.Shapes.Ellipse fence = new Windows.UI.Xaml.Shapes.Ellipse();
            //fence.Width = 30;
            //fence.Height = 30;
            //fence.Stroke = new SolidColorBrush(Colors.DarkOrange);
            //fence.StrokeThickness = 2;
            //MapControl.SetLocation(fence, geoposition.Coordinate.Point);
            //MapControl.SetNormalizedAnchorPoint(fence, new Point(0.5, 0.5));
            //mapWithMyLocation.Children.Add(fence);

        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void btnSaveParkingspace_Click(object sender, RoutedEventArgs e)
        {
            Geoposition geoposition = await getLocation();


            mapWithMyLocation.Center = geoposition.Coordinate.Point;
            MapIcon mapIcon = new MapIcon();
            //mapIcon.Image = RandomAccessStreamReference.CreateFromUri(
            //new Uri("ms-appx:///Assets/pin.png"));
            mapIcon.NormalizedAnchorPoint = new Point(0.25, 0.9);
            mapIcon.Location = geoposition.Coordinate.Point;
            mapIcon.Title = "Your parkingspace";
            mapWithMyLocation.MapElements.Add(mapIcon);
            mapWithMyLocation.ZoomLevel = 18;
            parkingspace = geoposition;

        }

        private async Task<Geoposition> getLocation()
        {
            Geolocator geolocator = new Geolocator();
            Geoposition geoposition = null;
            try
            {
                geoposition = await geolocator.GetGeopositionAsync();
            }
            catch (Exception ex)
            {
                // Handle errors like unauthorized access to location
                // services or no Internet access.
            }
            return geoposition;
        }



        private async void btnFindParkingspace_Click(object sender, RoutedEventArgs e)
        {
            Geoposition startPosition = await getLocation();
            if (parkingspace != null)
            {
                Geoposition endPosition = parkingspace;
            }
            else
            {
                var messageDialog = new MessageDialog("Geen parkingplaats bewaard.");
                await messageDialog.ShowAsync();
            }
            

            BasicGeoposition startLocation = new BasicGeoposition();
            startLocation.Latitude = startPosition.Coordinate.Point.Position.Latitude;
            startLocation.Longitude = startPosition.Coordinate.Point.Position.Longitude;
            Geopoint startPoint = new Geopoint(startLocation);

            BasicGeoposition endLocation = new BasicGeoposition();
            endLocation.Latitude = parkingspace.Coordinate.Point.Position.Latitude;
            endLocation.Longitude = parkingspace.Coordinate.Point.Position.Longitude;
            Geopoint endPoint = new Geopoint(endLocation);


            mapWithMyLocation.Center = startPosition.Coordinate.Point;
            MapIcon mapIcon = new MapIcon();
            //mapIcon.Image = RandomAccessStreamReference.CreateFromUri(
            //new Uri("ms-appx:///Assets/pin.png"));
            mapIcon.NormalizedAnchorPoint = new Point(0.25, 0.9);
            mapIcon.Location = startPosition.Coordinate.Point;
            mapIcon.Title = "You are here";
            mapWithMyLocation.MapElements.Add(mapIcon);
            mapWithMyLocation.ZoomLevel = 12;

            MapRouteFinderResult routeResult = await MapRouteFinder.GetWalkingRouteAsync(startPoint, endPoint);


            if (routeResult.Status == MapRouteFinderStatus.Success)
            {
                // Use the route to initialize a MapRouteView.
                MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                viewOfRoute.RouteColor = Colors.Blue;
                viewOfRoute.OutlineColor = Colors.Blue;
                // Add the new MapRouteView to the Routes collection
                // of the MapControl.
                mapWithMyLocation.Routes.Add(viewOfRoute);
                // Fit the MapControl to the route.
                await mapWithMyLocation.TrySetViewBoundsAsync(
                  routeResult.Route.BoundingBox,
                  null,
                  Windows.UI.Xaml.Controls.Maps.MapAnimationKind.Bow);
            }

            //tbTurnByTurn.Inlines.Add(new Run()
            //{
            //    Text = "Total estimated time (minutes) = "
            //    + routeResult.Route.EstimatedDuration.TotalMinutes.ToString("F1")
            //});
            //tbTurnByTurn.Inlines.Add(new LineBreak());
            //tbTurnByTurn.Inlines.Add(new Run()
            //{
            //    Text = "Total length (kilometers) = "
            //    + (routeResult.Route.LengthInMeters / 1000).ToString("F1")
            //});
            //tbTurnByTurn.Inlines.Add(new LineBreak());
            //// Display the directions.
            //tbTurnByTurn.Inlines.Add(new Run()
            //{
            //    Text = "DIRECTIONS"
            //});
            //tbTurnByTurn.Inlines.Add(new LineBreak());
            //// Loop through the legs and maneuvers.
            //int legCount = 0;
            //foreach (MapRouteLeg leg in routeResult.Route.Legs)
            //{
            //    foreach (MapRouteManeuver maneuver in leg.Maneuvers)
            //    {
            //        tbTurnByTurn.Inlines.Add(new Run()
            //        {
            //            Text = maneuver.InstructionText
            //        });
            //        tbTurnByTurn.Inlines.Add(new LineBreak());
            //    }
            //}
        }
    }
}
