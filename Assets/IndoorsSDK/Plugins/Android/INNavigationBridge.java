package com.unity3d.player;

import java.util.ArrayList;

import android.util.Log;
import pro.indoorsnavi.indoorssdk.core.INCore;
import pro.indoorsnavi.indoorssdk.core.INCoreConfiguration;
import pro.indoorsnavi.indoorssdk.dispatch.INDispatch;
import pro.indoorsnavi.indoorssdk.model.INApplication;
import pro.indoorsnavi.indoorssdk.model.INBuilding;
import pro.indoorsnavi.indoorssdk.model.INPosition;
import pro.indoorsnavi.indoorssdk.navigation.INNavigation;
import pro.indoorsnavi.indoorssdk.navigation.INNavigationDelegate;

public class INNavigationBridge implements INNavigationDelegate
{
    private static final String TAG = "INUnityNavigation";

    public static INBuilding Building = null;

    public void initialize(String params)
    {
        if (!INCore.isInitialized())
        {
            INDispatch.getInstance().executeOn(INDispatch.MAIN, () ->
            {
                INCoreConfiguration configuration = INCoreConfiguration.defaultConfiguration(UnityPlayer.currentActivity.getApplicationContext());
                INCore.initializeWithConfiguration(UnityPlayer.currentActivity.getApplicationContext(),configuration);
                UnityPlayer.UnitySendMessage("INNavigation", "OnNavigationInitialized", "");
            });
        }
    }

    public void loadBuilding(String params)
    {
        if (INCore.isInitialized())
        {
            UnityPlayer.UnitySendMessage("INNavigation", "OnBuildingLoadingStarted", "");

            INCore.getInstance().getService().authorizeApplicationWithClientId(
                    "LIUsaKGPnSePZfEGQL9yac3PLPtus4HrjcLpmKE5",
                    "VByRlq449otiO0mM0RfEJmjZnT8Ndll8WiPCi59Gr8RjnxW3HWeIPzNpyQtMAMLgasbUiEOYZyxvQdKH6xTFJno6YhBPWh1FxLwA35Q5YsQs8gUltX0zkopGPyhutEBa",
                    success ->
                    {
                        INCore.getInstance().getService().loadApplicationsWithCompletionBlock(
                                applications ->
                                {                 
                                    ArrayList apps = (ArrayList) applications;
                                     Log.i("POSITION", "loadApplicationsWithCompletionBlock");
                                    if (apps.size() > 0)
                                    {
                                        INApplication application = (INApplication) apps.get(0);
                                        INCore.getInstance().getService().loadBuildingsOfApplication(
                                                application, data ->
                                                {
                                                    ArrayList<INBuilding> buildings = (ArrayList) data.getData();
                                                    if (buildings.size() > 0)
                                                    {
                                                          Log.i("POSITION", "loadBuildingsOfApplication");
                                                        INBuilding building = buildings.get(0);
                                                        INCore.getInstance().getService().loadBuildingForNavigationOnlyWithoutGraphs(
                                                                building,
                                                                object ->
                                                                {
                                                                   Log.i("POSITION", "loadBuildingSome");
                                                                }, object ->
                                                                {
                                                                    Log.i("POSITION", "loadBuildingForNavigationOnly");
                                                                    Building = (INBuilding) object;
                                                                    UnityPlayer.UnitySendMessage("INNavigation", "OnBuildingLoaded", "");
                                                                });
                                                    }
                                                    else
                                                    {

                                                    }
                                                }, error -> {});
                                    }
                                    else
                                    {

                                    }
                                });
                    });
        }
    }

    public void startNavigation(String params)
    {
        if (INCore.isInitialized())
        {
            Log.i("POSITION", "startNavigation");

            INCore.getInstance().getNavigation().setBuilding(Building);
            INCore.getInstance().getNavigation().setNavigationDelegate(this);
            INCore.getInstance().getNavigation().startNavigation();
            UnityPlayer.UnitySendMessage("INNavigation", "OnNavigationStarted", "");
        }
    }

    public void stopNavigation(String params)
    {
        if (INCore.isInitialized())
        {
            INCore.getInstance().getNavigation().stopNavigation();
            UnityPlayer.UnitySendMessage("INNavigation", "OnNavigationStoped", "");
        }
    }

    @Override
    public void onPosition(INNavigation navigation, INPosition position)
    {
        INNavigationDelegate.super.onPosition(navigation, position);

        //simple format. json can be used instead
        int floorId = (int) position.FloorId;
        String dataString = position.X + " " + position.Y + " " + floorId;

        UnityPlayer.UnitySendMessage("INNavigation", "OnUpdateUserPosition", dataString);
    }

}
