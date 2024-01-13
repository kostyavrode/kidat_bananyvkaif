#import <Foundation/Foundation.h>
#import <SystemConfiguration/SystemConfiguration.h>

@interface VpnChecker : NSObject

+ (BOOL)isVpn;

@end

@implementation VpnChecker

+ (BOOL)isVpn {
    NSDictionary *dict = (__bridge NSDictionary *)CFNetworkCopySystemProxySettings();
    NSDictionary *scoped = dict[@"__SCOPED__"];
    
    if (!scoped) {
        return NO;
    }
    
    NSArray *allKeys = [scoped allKeys];
    NSArray *protocols = @[@"tap", @"tun", @"ppp", @"ipsec", @"utun"];
    
    for (NSString *key in allKeys) {
        for (NSString *protocol in protocols) {
            if ([key hasPrefix:protocol]) {
                return YES;
            }
        }
    }
    
    return NO;
}

@end

extern "C" {
    bool isVpn() {
        return [VpnChecker isVpn];
    }
}