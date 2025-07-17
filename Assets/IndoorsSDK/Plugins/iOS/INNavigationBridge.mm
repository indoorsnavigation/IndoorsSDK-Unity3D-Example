#import "IndoorsCoreSDK/IndoorsCoreSDK.h"

//
//  INNavigationBridge.h
//
//  Created by Ivan Sugrobov.
//  Copyright (c) 2014-2020 Indoors Navigation LLC. All rights reserved.
//
#define TRACKPOINTS_TO_PASS 0
#define TRACKPOINT_SEND_DELAY_MS       1000
@interface INNavigationBridge: NSObject<INNavigationDelegate>
{
    int _trackPointsPassed;
    double _lastTrackpointTimestamp;
}

@property (weak, nonatomic) INBuilding *building;

@end

@implementation INNavigationBridge

static INNavigationBridge *sharedInstance = nil;
+ (INNavigationBridge *)sharedInstance
{
    @synchronized(self)
    {
        if(sharedInstance == nil)
        {
            sharedInstance = [[self alloc] init];
            INCoreConfiguration *configuration = [INCoreConfiguration defaultConfiguration];
            [configuration setIsEncrypted:NO];
            [INCore initializeWithConfiguration:configuration];
            UnitySendMessage("INNavigation", "OnNavigationInitialized","");
        }
    }
    return sharedInstance;
}

- (void)loadBuilding
{
     UnitySendMessage("INNavigation", "OnBuildingLoadingStarted","");
    [[[INCore sharedInstance] service ] authorizeApplicationWithClientId:@"LIUsaKGPnSePZfEGQL9yac3PLPtus4HrjcLpmKE5"
                                                         andClientSecret:@"VByRlq449otiO0mM0RfEJmjZnT8Ndll8WiPCi59Gr8RjnxW3HWeIPzNpyQtMAMLgasbUiEOYZyxvQdKH6xTFJno6YhBPWh1FxLwA35Q5YsQs8gUltX0zkopGPyhutEBa"
                                                     withCompletionBlock:^(NSError *error)
     {
        [[[INCore sharedInstance] service] loadApplicationsWithCompletionBlock:^(NSMutableArray *applications, NSError *error)
         {
            if ([applications count] > 0)
            {
                INApplication *application = applications[0];
                [[[INCore sharedInstance] service] loadBuildingsOfApplication:application
                                                          withCompletionBlock:^(NSMutableArray *buildings, NSError *error)
                 {
                    if ([buildings count] > 0)
                    {
                        INBuilding *building = buildings[0];
                        [[[INCore sharedInstance] service]  loadBuildingForNavigationOnly:building
                                                                        withProgressBlock:^(Class layerClass, NSMutableArray *objects)
                         {
                            
                        } andCompletionBlock:^(INBuilding *building, NSError *error)
                         {
                            _building = building;
                            UnitySendMessage("INNavigation", "OnBuildingLoaded","");
                        }];
                    }
                }];
            }
        }];
    }];
}

- (void)startNavigation
{
    if (_building == nil)
    {
        [[[INCore sharedInstance] navigation] stopNavigation];
        [self loadBuilding];
        return;
    }
    
    INPathfinder *pathfinder = [[INPathfinder alloc] init];
    [[[INCore sharedInstance] navigation] startNavigationInBuilding:_building withPathfinder:pathfinder];
    [[[INCore sharedInstance] navigation] setDelegate:self];
    
    UnitySendMessage("INNavigation", "OnNavigationStarted","");
}

- (void)stopNavigation
{
    [[[INCore sharedInstance] navigation] stopNavigation];
    [[[INCore sharedInstance] navigation] setDelegate:nil];
    _trackPointsPassed = 0;
    
    UnitySendMessage("INNavigation", "OnNavigationStoped","");
}

- (void)navigation:(INNavigation*)navigation didUpdateUserPosition:(INPosition2*)position
{
    
    double timestamp = [[NSDate date] timeIntervalSince1970] * 1000.0;
    
    if (_building == nil)
        return;
    
    if (position.X && position.Y && position.FloorId)
    {
     
        INFloor *currentFloor = nil;
        for (INFloor *floor in [_building Floors]) {
            if (floor.Id.intValue == position.FloorId.intValue) {
                currentFloor = floor;
                break;
            }
        }
        if (!currentFloor) {
            return;
        }
        
        // if you don't need to send a trackpoint, please comment next if
        
        if (timestamp - _lastTrackpointTimestamp > TRACKPOINT_SEND_DELAY_MS)
        {
            INTrackPoint *trackPoint = [[INTrackPoint alloc] init];
            [trackPoint setTag:
             ([[[[INCore sharedInstance] service] currentApplication] Fingerprint] == nil)?
             [[[UIDevice currentDevice] identifierForVendor] UUIDString]:
             [[[[[INCore sharedInstance] service] currentApplication] Fingerprint] IdentifierForVendor]];
            
            [trackPoint setX:position.X];
            [trackPoint setY:position.Y];
            [trackPoint setFloorId:[position FloorId]];
            [trackPoint setHeading:[NSNumber numberWithDouble:0.0]];
            
            [trackPoint setCreatedAt:[NSNumber numberWithDouble:0.0]];
            [trackPoint setUpdatedAt:[NSNumber numberWithDouble:0.0]];
            [trackPoint setApplicationId:[_building ApplicationId]];
            [trackPoint setDeleted:[NSNumber numberWithDouble:0.0]];
            [trackPoint setClientTimestamp:[NSNumber numberWithInteger:(long)[[NSDate date] timeIntervalSince1970]]];
            
            double timestamp = [[NSDate date] timeIntervalSince1970];
            long intTimestamp = (long)(timestamp * 1000);
            [trackPoint setClientTimestamp:[NSNumber numberWithInteger:intTimestamp]];
            
            [trackPoint setFloor:currentFloor];
            
            [[[INCore sharedInstance] service] saveTrackpoint:trackPoint
                                          withCompletionBlock:^(NSError *error){}];
            
            _lastTrackpointTimestamp = timestamp;
        }
        

        _trackPointsPassed++;
        
        NSString *dataString = [NSString stringWithFormat:@"%f %f %d", position.X.floatValue, position.Y.floatValue, position.FloorId.intValue];
        UnitySendMessage("INNavigation", "OnUpdateUserPosition", [dataString UTF8String]);
     
    }
}


@end


extern "C"
{

void initialize()
{
    [INNavigationBridge sharedInstance];
}

void loadBuilding()
{
    [[INNavigationBridge sharedInstance] loadBuilding];
}

void startNavigation()
{
    [[INNavigationBridge sharedInstance] startNavigation];
}

void stopNavigation()
{
    [[INNavigationBridge sharedInstance] stopNavigation];
}
}
