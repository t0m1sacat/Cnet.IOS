//
//  OSNewTimesheetViewController.m
//  College Nannies and Tutors Layouts
//
//  Created by kristian.lien on 3/18/14.
//  Copyright (c) 2014 Onsharp. All rights reserved.
//

#import "OSNewTimesheetViewController.h"

@interface OSNewTimesheetViewController ()

@end

@implementation OSNewTimesheetViewController

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        // Custom initialization
    }
    return self;
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    // Do any additional setup after loading the view.
    
    [[recapTextView layer] setBorderColor:[[UIColor lightGrayColor] CGColor]];
    [[recapTextView layer] setBorderWidth:1.0];
    [[recapTextView layer] setCornerRadius:1.5];
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

/*
#pragma mark - Navigation

// In a storyboard-based application, you will often want to do a little preparation before navigation
- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender
{
    // Get the new view controller using [segue destinationViewController].
    // Pass the selected object to the new view controller.
}
*/

@end
