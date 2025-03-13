// src/jasmine-fix.d.ts
declare namespace jasmine {
    interface Matchers<T> {
      toBeTruthy(): boolean;
      toBeFalse(): boolean;
      toBeTrue(): boolean;
      toBe(expected: any): boolean;
      toEqual(expected: any): boolean;
      toHaveBeenCalled(): boolean;
      toHaveBeenCalledWith(...params: any[]): boolean;
      toHaveBeenCalledTimes(count: number): boolean;
      toContain(expected: any): boolean;
    }
  }