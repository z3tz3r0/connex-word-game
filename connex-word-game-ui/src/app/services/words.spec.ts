import { TestBed } from '@angular/core/testing';

import { Words } from './words';

describe('Words', () => {
  let service: Words;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Words);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
