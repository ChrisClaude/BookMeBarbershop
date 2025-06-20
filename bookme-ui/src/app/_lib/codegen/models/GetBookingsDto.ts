/* tslint:disable */
/* eslint-disable */
/**
 * BookMeAPI | v1
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.0.0
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */

import { mapValues } from '../runtime';
/**
 * 
 * @export
 * @interface GetBookingsDto
 */
export interface GetBookingsDto {
    /**
     * 
     * @type {Date}
     * @memberof GetBookingsDto
     */
    fromDateTime?: Date;
    /**
     * 
     * @type {number}
     * @memberof GetBookingsDto
     */
    bookingStatus?: number | null;
}

/**
 * Check if a given object implements the GetBookingsDto interface.
 */
export function instanceOfGetBookingsDto(value: object): value is GetBookingsDto {
    return true;
}

export function GetBookingsDtoFromJSON(json: any): GetBookingsDto {
    return GetBookingsDtoFromJSONTyped(json, false);
}

export function GetBookingsDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): GetBookingsDto {
    if (json == null) {
        return json;
    }
    return {
        
        'fromDateTime': json['fromDateTime'] == null ? undefined : (new Date(json['fromDateTime'])),
        'bookingStatus': json['bookingStatus'] == null ? undefined : json['bookingStatus'],
    };
}

export function GetBookingsDtoToJSON(json: any): GetBookingsDto {
    return GetBookingsDtoToJSONTyped(json, false);
}

export function GetBookingsDtoToJSONTyped(value?: GetBookingsDto | null, ignoreDiscriminator: boolean = false): any {
    if (value == null) {
        return value;
    }

    return {
        
        'fromDateTime': value['fromDateTime'] == null ? undefined : ((value['fromDateTime']).toISOString()),
        'bookingStatus': value['bookingStatus'],
    };
}

