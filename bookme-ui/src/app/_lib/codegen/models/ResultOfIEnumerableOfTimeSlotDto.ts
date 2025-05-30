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
import type { TimeSlotDto } from './TimeSlotDto';
import {
    TimeSlotDtoFromJSON,
    TimeSlotDtoFromJSONTyped,
    TimeSlotDtoToJSON,
    TimeSlotDtoToJSONTyped,
} from './TimeSlotDto';

/**
 * 
 * @export
 * @interface ResultOfIEnumerableOfTimeSlotDto
 */
export interface ResultOfIEnumerableOfTimeSlotDto {
    /**
     * 
     * @type {Array<TimeSlotDto>}
     * @memberof ResultOfIEnumerableOfTimeSlotDto
     */
    value?: Array<TimeSlotDto> | null;
    /**
     * 
     * @type {boolean}
     * @memberof ResultOfIEnumerableOfTimeSlotDto
     */
    isSuccess?: boolean;
    /**
     * 
     * @type {boolean}
     * @memberof ResultOfIEnumerableOfTimeSlotDto
     */
    isFailure?: boolean;
    /**
     * 
     * @type {Array<Error>}
     * @memberof ResultOfIEnumerableOfTimeSlotDto
     */
    errors?: Array<Error> | null;
    /**
     * 
     * @type {number}
     * @memberof ResultOfIEnumerableOfTimeSlotDto
     */
    successType?: number;
}

/**
 * Check if a given object implements the ResultOfIEnumerableOfTimeSlotDto interface.
 */
export function instanceOfResultOfIEnumerableOfTimeSlotDto(value: object): value is ResultOfIEnumerableOfTimeSlotDto {
    return true;
}

export function ResultOfIEnumerableOfTimeSlotDtoFromJSON(json: any): ResultOfIEnumerableOfTimeSlotDto {
    return ResultOfIEnumerableOfTimeSlotDtoFromJSONTyped(json, false);
}

export function ResultOfIEnumerableOfTimeSlotDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): ResultOfIEnumerableOfTimeSlotDto {
    if (json == null) {
        return json;
    }
    return {
        
        'value': json['value'] == null ? undefined : ((json['value'] as Array<any>).map(TimeSlotDtoFromJSON)),
        'isSuccess': json['isSuccess'] == null ? undefined : json['isSuccess'],
        'isFailure': json['isFailure'] == null ? undefined : json['isFailure'],
        'errors': json['errors'] == null ? undefined : json['errors'],
        'successType': json['successType'] == null ? undefined : json['successType'],
    };
}

export function ResultOfIEnumerableOfTimeSlotDtoToJSON(json: any): ResultOfIEnumerableOfTimeSlotDto {
    return ResultOfIEnumerableOfTimeSlotDtoToJSONTyped(json, false);
}

export function ResultOfIEnumerableOfTimeSlotDtoToJSONTyped(value?: ResultOfIEnumerableOfTimeSlotDto | null, ignoreDiscriminator: boolean = false): any {
    if (value == null) {
        return value;
    }

    return {
        
        'value': value['value'] == null ? undefined : ((value['value'] as Array<any>).map(TimeSlotDtoToJSON)),
        'isSuccess': value['isSuccess'],
        'isFailure': value['isFailure'],
        'errors': value['errors'],
        'successType': value['successType'],
    };
}

