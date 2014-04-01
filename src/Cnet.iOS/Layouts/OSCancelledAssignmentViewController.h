//
//  OSCancelledAssignmentViewController.h
//  College Nannies and Tutors Layouts
//
//  Created by kristian.lien on 3/10/14.
//  Copyright (c) 2014 Onsharp. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "OSFamilyCell.h"
#import "OSStartEndCell.h"
#import "OSTimesCell.h"
#import "OSLocationCell.h"
#import "OSChildrenCell.h"
#import "OSInfoCell.h"
#import "OSDetailsCell.h"

@interface OSCancelledAssignmentViewController : UIViewController <UITableViewDataSource, UITableViewDelegate>
{
    NSArray *testArray;
}


@end
