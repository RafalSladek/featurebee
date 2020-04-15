FeatureBee
==========

FeatureBee helps you to release features without fears and pains. It´s the piece of puzzle which makes your Continuous Deplyoment perfect. FeatureBee decouples code releases from feature releases and makes it easy to rollout new features step-by-step.

For more details read [here](http://pgarbe.github.io/blog/2014/01/16/the-fear-of-new-features/) and [here](http://www.codeproject.com/Articles/727298/Getting-your-features-out)

![FeatureBee CI Build Status](https://www.myget.org/BuildSource/Badge/featurebee-ci?identifier=24682329-ee15-41cc-ad5e-922ae684f261)

You can see the current work in progress for the tasks on FeatureBee [here](https://trello.com/b/jrHcCKQa/featurebee).
Contact us if you want to help.

## FeatureBee Server 

The Server distributes the feature state to all registered clients, and is the ui for whowever is editing the feature state.

![FeatureBee Server](https://github.com/AutoScout24/FeatureBee/raw/master/documentation/images/Feature%20Bee%20-%20Server.png)

## FeatureBee Client

The client is the application where you implement the features. The features are toggled in an IF-Statement:

    if (Feature.IsEnabled("My Feature"))
    {
      ViewBag.Message = "Congratulations!";
    }

You can unit test this by injecting your own evaluator: 

    Feature.InjectEvaluator(featureName => true);
    if (Feature.IsEnabled("My Feature")) // always true
    {
      ViewBag.Message = "Congratulations!";
    }

## FeatureBee TrayIcon

The TrayIcon is displayed on the page, and can be used to turn features off and on for the current user.

![Feature Bee Tray](https://github.com/AutoScout24/FeatureBee/raw/master/documentation/images/Feature%20Bee%20-%20Tray%20Icon.png)


## How to contribute

1. Check the [issues](https://github.com/AutoScout24/FeatureBee/issues) for stuff that needs to be done.
2. Fork the repository
3. Make your changes (in it´s own branch)
4. Send PullRequest

[See the wiki for more information](https://github.com/AutoScout24/FeatureBee/wiki/) 
