#import <Foundation/Foundation.h>

extern "C" {
        const char* timeZoneName() {
        NSString* timeZoneName = [[NSTimeZone systemTimeZone] name];
        return strdup([timeZoneName UTF8String]);
    }

}